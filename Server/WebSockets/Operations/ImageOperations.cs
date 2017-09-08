using API.Domain;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Input;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        static ImageOperations() {
            serializerSettings = new JsonSerializerSettings() {
                Converters = { new StringEnumConverter() }
            };
        }

        public ImageOperations(IImageService imageService, IFigureIdService figureIdService, IAuthorizationService authorizationService) {
            _imageService = imageService;
            _figureIdService = figureIdService;
            _authorizationService = authorizationService;
        }

        public async Task CreateImage(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject payload) {
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(stringWebSocket.User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                return;//TODO REVER
            }

            InCreateWSImage inImage = payload.ToObject<InCreateWSImage>();

            if(inImage.BoardId != boardId) {
                return;//TODO REVER
            }

            var validationResults = new List<ValidationResult>();
            if(!Validator.TryValidateObject(inImage, new ValidationContext(inImage), validationResults, true)) {
                return;//TODO REVER
            }

            IFigureIdGenerator idGen = await _figureIdService.GetOrCreateFigureIdGeneratorAsync(boardId);
            long id = idGen.NewId();

            Image image = new Image { Id = id }.In(inImage);

            Task store = _imageService.CreateAsync(image, autoGenerateId: false);
            OperationUtils.ResolveTaskContinuation(store);

            Task response = stringWebSocket.SendAsync(
                new {
                    type = OperationType.CREATE_IMAGE,
                    payload = new { id = id, tempId = inImage.TempId }
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
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(stringWebSocket.User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                return;//TODO REVER
            }

            InUpdateImage inImage = jPayload.ToObject<InUpdateImage>();

            if(inImage.BoardId != boardId) {
                return;//TODO REVER
            }

            var validationResults = new List<ValidationResult>();
            if(!Validator.TryValidateObject(inImage, new ValidationContext(inImage), validationResults, true)) {
                return;//TODO REVER
            }

            Image image = await _imageService.GetAsync(inImage.Id.Value, boardId);
            if(image == null) {
                return;//TODO REVER
            }

            image.In(inImage);

            Task store = _imageService.UpdateAsync(image);
            OperationUtils.ResolveTaskContinuation(store);

            await session.BroadcastAsync(
                new {
                    type = OperationType.ALTER_IMAGE,
                    payload = new { figure = image.Out() }
                },
                serializerSettings
            );
        }

        public async Task DeleteImage(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject jPayload) {
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(stringWebSocket.User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                return;//TODO REVER
            }

            if(!(jPayload.TryGetValue("id", System.StringComparison.OrdinalIgnoreCase, out JToken payload_id) && payload_id.Type == JTokenType.Integer)) {
                return;//TODO REVER
            }
            long id = payload_id.Value<long>();

            Image image = await _imageService.GetAsync(id, boardId);
            if(image == null) {
                return;//TODO REVER
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
