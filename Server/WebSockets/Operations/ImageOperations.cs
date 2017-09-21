using QIP.Domain;
using QIP.Public;
using QIP.Public.IServices;
using QIP.Public.ServicesExceptions;
using QIP.Authorization;
using QIP.Authorization.Extensions;
using QIP.Authorization.Resources;
using QIP.IODomain.Extensions;
using QIP.IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using QIP.WebSockets.Extensions;
using QIP.WebSockets.Models;
using QIP.WebSockets.StringWebSockets;

namespace QIP.WebSockets.Operations {
    public class ImageOperations {
        private static readonly JsonSerializerSettings serializerSettings;

        private readonly IImageService _imageService;
        private readonly IFigureIdService _figureIdService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<ImageOperations> _logger;

        static ImageOperations() {
            serializerSettings = new JsonSerializerSettings() {
                Converters = { new StringEnumConverter() }
            };
        }

        public ImageOperations(IImageService imageService, IFigureIdService figureIdService, IAuthorizationService authorizationService, ILogger<ImageOperations> logger) {
            _imageService = imageService;
            _figureIdService = figureIdService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        public async Task CreateImage(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject payload) {
            ClaimsPrincipal user = stringWebSocket.User;
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(user, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.InsertWSImageNotAuthorized, "CreateImage (Board {boardId}) NOT AUTHORIZED {userId}", boardId, user.GetNameIdentifier());
                return;
            }

            InCreateWSImage inImage = payload.ToObject<InCreateWSImage>();

            var validationResults = new List<ValidationResult>();
            if(!Validator.TryValidateObject(inImage, new ValidationContext(inImage), validationResults, true)) {
                _logger.LogDebug(LoggingEvents.InsertWSImageInvalidModel, "CreateImage (Board {boardId}) INVALID MODEL", boardId);
                return;
            }

            if(inImage.BoardId != boardId) {
                _logger.LogDebug(LoggingEvents.InsertWSImageWrongBoardId, "CreateImage (Board {boardId}) WRONG BOARD ID {otherBoardId}", boardId, inImage.BoardId);
                return;
            }

            IFigureIdGenerator idGen = await _figureIdService.GetOrCreateFigureIdGeneratorAsync(boardId);
            Image image = new Image { Id = idGen.NewId() }.In(inImage);

            Task store = _imageService.CreateAsync(image, autoGenerateId: false);
            if(store.IsFaulted) {
                ReadOnlyCollection<Exception> exceptions = store.Exception.InnerExceptions;
                Exception serviceException = exceptions.First();
                if(exceptions.Count == 1 && serviceException.GetType() == typeof(ServiceException)) {
                    _logger.LogError(LoggingEvents.InsertWSImageUnexpectedServiceError, serviceException, "CreateImage (Board {boardId}) UNEXPECTED SERVICE ERROR", boardId);
                }
                else {
                    _logger.LogError(LoggingEvents.InsertWSImageUnexpectedError, store.Exception, "CreateImage (Board {boardId}) UNEXPECTED ERROR", boardId);
                }
                return;
            }

            Task messages = SendInsertMessages(stringWebSocket, session, image.Out(), inImage.TempId, inImage.PersistLocalBoard);
            try {
                await store;
                _logger.LogInformation(LoggingEvents.InsertWSImage, "Image {id} of Board {boardId} Created", image.Id, boardId);

                await messages;
            }
            catch(Exception e) {
                _logger.LogError(LoggingEvents.InsertWSImageUnexpectedError, e, "CreateImage (Board {boardId}) UNEXPECTED ERROR", boardId);
            }
        }

        private static Task SendInsertMessages(StringWebSocket stringWebSocket, IStringWebSocketSession session, OutImage image, long tempId, bool persistLocalBoard) {
            dynamic sendPayload;
            if(persistLocalBoard)
                sendPayload = new { figure = image };
            else
                sendPayload = new { id = image.Id, tempId = tempId };

            Task response = stringWebSocket.SendAsync(
                new {
                    type = OperationType.CREATE_IMAGE,
                    payload = sendPayload
                },
               serializerSettings
            );

            Task broadcast = session.BroadcastAsync(
               new {
                   type = OperationType.CREATE_IMAGE,
                   payload = new { figure = image }
               },
               serializerSettings
            );

            return Task.WhenAll(response, broadcast);
        }

        public async Task UpdateImage(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject jPayload) {
            ClaimsPrincipal user = stringWebSocket.User;
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(user, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.UpdateWSImageNotAuthorized, "UpdateImage (Board {boardId}) NOT AUTHORIZED {userId}", boardId, user.GetNameIdentifier());
                return;
            }

            InUpdateWSImage inImage = jPayload.ToObject<InUpdateWSImage>();

            var validationResults = new List<ValidationResult>();
            if(!Validator.TryValidateObject(inImage, new ValidationContext(inImage), validationResults, true)) {
                _logger.LogDebug(LoggingEvents.UpdateWSImageInvalidModel, "UpdateImage (Board {boardId}) INVALID MODEL", boardId);
                return;
            }

            if(inImage.BoardId != boardId) {
                _logger.LogDebug(LoggingEvents.UpdateWSImageWrongBoardId, "UpdateImage (Board {boardId}) WRONG BOARD ID {otherBoardId}", boardId, inImage.BoardId);
                return;
            }

            Image image = await _imageService.GetAsync(inImage.Id.Value, boardId);
            if(image == null) {
                _logger.LogWarning(LoggingEvents.UpdateWSImageNotFound, "UpdateImage (Board {boardId}) NOT FOUND", boardId);
                return;
            }

            Task store = _imageService.UpdateAsync(image.In(inImage));
            if(store.IsFaulted) {
                ReadOnlyCollection<Exception> exceptions = store.Exception.InnerExceptions;
                Exception serviceException = exceptions.First();
                if(exceptions.Count == 1 && serviceException.GetType() == typeof(ServiceException)) {
                    _logger.LogError(LoggingEvents.UpdateWSImageUnexpectedServiceError, serviceException, "UpdateImage {id} (Board {boardId}) UNEXPECTED SERVICE ERROR", image.Id, boardId);
                }
                else {
                    _logger.LogError(LoggingEvents.UpdateWSImageUnexpectedError, store.Exception, "UpdateImage {id} (Board {boardId}) UNEXPECTED ERROR", image.Id, boardId);
                }
                return;
            }

            Task messages = SendUpdateMessages(session, image.Out(), inImage.IsScaling);
            try {
                await store;
                _logger.LogInformation(LoggingEvents.InsertWSImage, "Image {id} of Board {boardId} Updated", image.Id, boardId);

                await messages;
            }
            catch(Exception e) {
                _logger.LogError(LoggingEvents.InsertWSImageUnexpectedError, e, "UpdateImage {id} (Board {boardId}) UNEXPECTED ERROR", image.Id, boardId);
            }
        }

        private static Task SendUpdateMessages(IStringWebSocketSession session, OutImage image, string isScaling) {
            return session.BroadcastAsync(
                new {
                    type = OperationType.ALTER_IMAGE,
                    isScaling = isScaling,
                    payload = new { figure = image }
                },
                serializerSettings
            );
        }

        public async Task DeleteImage(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject jPayload) {
            ClaimsPrincipal user = stringWebSocket.User;
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(user, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.DeleteWSImageNotAuthorized, "DeleteImage (Board {boardId}) NOT AUTHORIZED {userId}", boardId, user.GetNameIdentifier());
                return;
            }

            if(!(jPayload.TryGetValue("id", StringComparison.OrdinalIgnoreCase, out JToken payload_id) && payload_id.Type == JTokenType.Integer)) {
                _logger.LogDebug(LoggingEvents.DeleteWSImageInvalidModel, "DeleteImage (Board {boardId}) INVALID MODEL", boardId);
                return;
            }
            long id = payload_id.Value<long>();

            Image image = await _imageService.GetAsync(id, boardId);
            if(image == null) {
                _logger.LogWarning(LoggingEvents.DeleteWSImageNotFound, "DeleteImage (Board {boardId}) NOT FOUND", boardId);
                return;
            }

            Task store = _imageService.DeleteAsync(id, boardId);
            if(store.IsFaulted) {
                ReadOnlyCollection<Exception> exceptions = store.Exception.InnerExceptions;
                Exception serviceException = exceptions.First();
                if(exceptions.Count == 1 && serviceException.GetType() == typeof(ServiceException)) {
                    _logger.LogError(LoggingEvents.DeleteWSImageUnexpectedServiceError, serviceException, "DeleteImage {id} (Board {boardId}) UNEXPECTED SERVICE ERROR", image.Id, boardId);
                }
                else {
                    _logger.LogError(LoggingEvents.DeleteWSImageUnexpectedError, store.Exception, "DeleteImage {id} (Board {boardId}) UNEXPECTED ERROR", image.Id, boardId);
                }
                return;
            }

            Task messages = SendDeleteMessages(session, id);
            try {
                await store;
                _logger.LogInformation(LoggingEvents.DeleteWSImage, "Image {id} of Board {boardId} Deleted", image.Id, boardId);

                await messages;
            }
            catch(Exception e) {
                _logger.LogError(LoggingEvents.DeleteWSImageUnexpectedError, e, "DeleteImage {id} (Board {boardId}) UNEXPECTED ERROR", image.Id, boardId);
            }
        }

        private static Task SendDeleteMessages(IStringWebSocketSession session, long id) {
            return session.BroadcastAsync(
                new {
                    type = OperationType.DELETE_IMAGE,
                    payload = new { id = id }
                },
                serializerSettings
            );
        }
    }
}
