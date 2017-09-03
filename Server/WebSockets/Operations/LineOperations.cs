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
    public class LineOperations {// TODO Replace Repositories by Services
        private readonly ILineRepository _lineRepository;
        private readonly IFigureIdService _figureIdService;
        private readonly IAuthorizationService _authorizationService;

        public LineOperations(ILineRepository lineRepository, IFigureIdService figureIdService, IAuthorizationService authorizationService) {
            _lineRepository = lineRepository;
            _figureIdService = figureIdService;
            _authorizationService = authorizationService;
        }

        public async Task CreateLine(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject payload) {//TODO Rever se não pomos os checks aos ids e outros como nos controlers
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

            InLine inLine = payload.ToObject<InLine>();
            inLine.Id = id;

            Line line = new Line(boardId, id).In(inLine);
            Task store = _lineRepository.AddAsync(line);
            OperationUtils.ResolveTaskContinuation(store);

            string jsonRes = JsonConvert.SerializeObject(
                new {
                    type = Models.Action.CREATE_LINE.ToString(),
                    payload = new { id = id, tempId = tempId }
                }
            );
            Task response = stringWebSocket.SendAsync(jsonRes);

            string jsonBroadcast = JsonConvert.SerializeObject(
                new {
                    type = Models.Action.CREATE_LINE.ToString(),
                    payload = new { figure = inLine } // Usado o InLine porque os WebSockets estão a servir de espelho ao enviado pelo cliente
                }
            );
            Task broadcast = session.BroadcastAsync(jsonBroadcast);

            await Task.WhenAll(response, broadcast);
        }

        public async Task UpdateLine(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject payload) {
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(stringWebSocket.User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                return;//TODO REVER
            }

            long id = payload["id"].Value<long>();

            int offsetPoint = payload["offsetPoint"].Value<int>();

            InLine inLine = payload.ToObject<InLine>();

            Task store = StoreUpdateLine(id, boardId, inLine);
            OperationUtils.ResolveTaskContinuation(store);

            string jsonBroadcast = JsonConvert.SerializeObject(
                new {
                    type = Models.Action.ALTER_LINE.ToString(),
                    payload = new {
                        offsetPoint = offsetPoint,
                        figure = inLine // Usado o InLine porque os WebSockets estão a servir de espelho ao enviado pelo cliente
                    }
                }
            );
            await session.BroadcastAsync(jsonBroadcast);
        }

        private async Task StoreUpdateLine(long id, long boardId, InLine inLine) {
            Line line = await _lineRepository.FindAsync(id, boardId);
            if(line == null) {
                return;//TODO REVER
            }

            line.In(inLine);

            await _lineRepository.UpdateAsync(line);
        }

        public async Task DeleteLine(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject payload) {
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(stringWebSocket.User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                return;//TODO REVER
            }

            long id = payload["id"].Value<long>();

            Task store = StoreDeleteLine(id, boardId);
            OperationUtils.ResolveTaskContinuation(store);

            string jsonBroadcast = JsonConvert.SerializeObject(
                new {
                    type = Models.Action.DELETE_LINE.ToString(),
                    payload = new { id = id }
                }
            );
            await session.BroadcastAsync(jsonBroadcast);
        }

        private async Task StoreDeleteLine(long id, long boardId) {
            Line line = await _lineRepository.FindAsync(id, boardId);
            if(line == null) {
                return;//TODO REVER
            }
            await _lineRepository.RemoveAsync(id, boardId);
        }
    }
}
