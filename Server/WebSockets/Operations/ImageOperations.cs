using API.Domain;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using API.Services;
using API.Services.Extensions;
using Authorization;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using WebSockets.StringWebSockets;

namespace WebSockets.Operations {
    public class ImageOperations {// TODO Replace Repositories by Services
        private readonly IImageRepository _imageRepository;
        private readonly IFigureIdService _figureIdService;
        private readonly IAuthorizationService _authorizationService;

        public ImageOperations(IImageRepository imageRepository, IFigureIdService figureIdService, IAuthorizationService authorizationService) {
            _imageRepository = imageRepository;
            _figureIdService = figureIdService;
            _authorizationService = authorizationService;
        }

        public async Task CreateImage(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject payload) {//TODO Rever se não pomos os checks aos ids e outros como nos controlers
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(stringWebSocket.User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                return;//TODO REVER
            }

            if(!(payload.TryGetValue("tempId", StringComparison.OrdinalIgnoreCase, out JToken payload_tempId) && payload_tempId.Type == JTokenType.Integer)) {
                return;//TODO REVER
            }
            long tempId = payload["tempId"].Value<long>();

            IFigureIdGenerator idGen = await _figureIdService.GetOrCreateFigureIdGeneratorAsync(boardId);
            long id = idGen.NewId();

            InImage inImage = payload.ToObject<InImage>();
            inImage.Id = id;
            Image image = new Image(boardId, id).In(inImage);
            Task store = _imageRepository.AddAsync(image);
            OperationUtils.ResolveTaskContinuation(store);

            string jsonRes = JsonConvert.SerializeObject(
                new {
                    type = Models.Action.CREATE_IMAGE.ToString(),
                    payload = new { id = id, tempId = tempId }
                }
            );
            
            // se há uma flag persistLocalBoard, quer dizer que esta figura foi criada para persistir um board local. Dessa forma, é necessário fazer espelho da figura
            if((payload.TryGetValue("persistLocalBoard", StringComparison.OrdinalIgnoreCase, out JToken payload_persistLocalBoard) && payload_tempId.Type == JTokenType.Integer)) {
                jsonRes = JsonConvert.SerializeObject(
                    new {
                        type = Models.Action.CREATE_LINE.ToString(),
                        payload = new { figure = inImage }
                    }
                );
            }
            Task response =  stringWebSocket.SendAsync(jsonRes);

            string jsonBroadcast = JsonConvert.SerializeObject(
                new {
                    type = Models.Action.CREATE_IMAGE.ToString(),
                    payload = new { figure = inImage } // Usado o InImage porque os WebSockets estão a servir de espelho ao enviado pelo cliente
                }
            );
            Task broadcast = session.BroadcastAsync(jsonBroadcast);

            await Task.WhenAll(response, broadcast);
        }

        public async Task UpdateImage(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject payload) {
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(stringWebSocket.User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                return;//TODO REVER
            }

            long id = payload["id"].Value<long>();

            InImage inImage = payload.ToObject<InImage>();

            Task store = StoreUpdateImage(id, boardId, inImage);
            OperationUtils.ResolveTaskContinuation(store);

            string jsonBroadcast = JsonConvert.SerializeObject(
                new {
                    type = Models.Action.ALTER_IMAGE.ToString(),
                    payload = new { figure = inImage } // Usado o InImage porque os WebSockets estão a servir de espelho ao enviado pelo cliente
                }
            );
            await session.BroadcastAsync(jsonBroadcast);
        }

        private async Task StoreUpdateImage(long id, long boardId, InImage inImage) {
            Image image = await _imageRepository.FindAsync(id, boardId);
            if(image == null) {
                return;//TODO REVER
            }

            image.In(inImage);

            await _imageRepository.UpdateAsync(image);
        }

        public async Task DeleteImage(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject payload) {
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(stringWebSocket.User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                return;//TODO REVER
            }

            long id = payload["id"].Value<long>();

            Task store = StoreDeleteImage(id, boardId);
            OperationUtils.ResolveTaskContinuation(store);

            string jsonBroadcast = JsonConvert.SerializeObject(
                new {
                    type = Models.Action.DELETE_IMAGE.ToString(),
                    payload = new {id = id}
                }
            );
            await session.BroadcastAsync(jsonBroadcast);
        }

        private async Task StoreDeleteImage(long id, long boardId) {
            Image image = await _imageRepository.FindAsync(id, boardId);
            if(image == null) {
                return;//TODO REVER
            }
            await _imageRepository.RemoveAsync(id, boardId);
        }
    }
}
