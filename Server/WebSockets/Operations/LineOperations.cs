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

            InCreateLine inLine = payload.ToObject<InCreateLine>();

            Line line = new Line { Id = id, BoardId = boardId }.In(inLine);
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
                    payload = new { figure = line.Out() }
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

            InUpdateLine inLine = payload.ToObject<InUpdateLine>();

            Task store = StoreUpdateLine(id, boardId, inLine);
            OperationUtils.ResolveTaskContinuation(store);

            string jsonBroadcast = JsonConvert.SerializeObject(
                new {
                    type = Models.Action.ALTER_LINE.ToString(),
                    payload = new {
                        offsetPoint = offsetPoint,
                        figure = inLine // TODO Usar Out
                    }
                }
            );
            await session.BroadcastAsync(jsonBroadcast);
        }

        private async Task StoreUpdateLine(long id, long boardId, InUpdateLine inLine) {
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
