using API.Domain;
using API.Interfaces;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using WebSockets.Extensions;
using WebSockets.Models;
using WebSockets.StringWebSockets;

namespace WebSockets.Operations {
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

            if(inImage.BoardId != boardId) {
                _logger.LogDebug(LoggingEvents.InsertWSImageWrongBoardId, "CreateImage (Board {boardId}) WRONG BOARD ID {otherBoardId}", boardId, inImage.BoardId);
                return;
            }

            var validationResults = new List<ValidationResult>();
            if(!Validator.TryValidateObject(inImage, new ValidationContext(inImage), validationResults, true)) {
                _logger.LogDebug(LoggingEvents.InsertWSImageInvalidModel, "CreateImage (Board {boardId}) INVALID MODEL", boardId);
                return;
            }

            IFigureIdGenerator idGen = await _figureIdService.GetOrCreateFigureIdGeneratorAsync(boardId);
            long id = idGen.NewId();

            Image image = new Image { Id = id }.In(inImage);

            Task store = _imageService.CreateAsync(image, autoGenerateId: false);
            OperationUtils.ResolveTaskContinuation(store);

            dynamic sendPayload;
            if(inImage.PersistLocalBoard)
                sendPayload = new { figure = image.Out() };
            else
                sendPayload = new { id = id, tempId = inImage.TempId };

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
                   payload = new { figure = image.Out() }
               },
               serializerSettings
            );

            await Task.WhenAll(response, broadcast);
        }

        public async Task UpdateImage(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject jPayload) {
            ClaimsPrincipal user = stringWebSocket.User;
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(user, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.UpdateWSImageNotAuthorized, "UpdateImage (Board {boardId}) NOT AUTHORIZED {userId}", boardId, user.GetNameIdentifier());
                return;
            }

            InUpdateWSImage inImage = jPayload.ToObject<InUpdateWSImage>();

            if(inImage.BoardId != boardId) {
                _logger.LogDebug(LoggingEvents.UpdateWSImageWrongBoardId, "UpdateImage (Board {boardId}) WRONG BOARD ID {otherBoardId}", boardId, inImage.BoardId);
                return;
            }

            var validationResults = new List<ValidationResult>();
            if(!Validator.TryValidateObject(inImage, new ValidationContext(inImage), validationResults, true)) {
                _logger.LogDebug(LoggingEvents.UpdateWSImageInvalidModel, "UpdateImage (Board {boardId}) INVALID MODEL", boardId);
                return;
            }

            Image image = await _imageService.GetAsync(inImage.Id.Value, boardId);
            if(image == null) {
                _logger.LogWarning(LoggingEvents.UpdateWSImageNotFound, "UpdateImage {id} (Board {boardId}) NOT FOUND", image.Id, boardId);
                return;
            }

            image.In(inImage);

            Task store = _imageService.UpdateAsync(image);
            OperationUtils.ResolveTaskContinuation(store);

            await session.BroadcastAsync(
                new {
                    type = OperationType.ALTER_IMAGE,
                    isScaling = inImage.IsScaling,
                    payload = new { figure = image.Out() }
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

            if(!(jPayload.TryGetValue("id", System.StringComparison.OrdinalIgnoreCase, out JToken payload_id) && payload_id.Type == JTokenType.Integer)) {
                _logger.LogDebug(LoggingEvents.DeleteWSImageInvalidModel, "DeleteImage (Board {boardId}) INVALID MODEL", boardId);
                return;
            }
            long id = payload_id.Value<long>();

            Image image = await _imageService.GetAsync(id, boardId);
            if(image == null) {
                _logger.LogWarning(LoggingEvents.DeleteWSImageNotFound, "DeleteImage {id} (Board {boardId}) NOT FOUND", image.Id, boardId);
                return;
            }

            Task store = _imageService.DeleteAsync(id, boardId);
            OperationUtils.ResolveTaskContinuation(store);

            await session.BroadcastAsync(
                new {
                    type = OperationType.DELETE_IMAGE,
                    payload = new { id = id }
                },
                serializerSettings
            );
        }
    }
}
