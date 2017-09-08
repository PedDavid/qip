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
    public class LineOperations {
        private static readonly JsonSerializerSettings serializerSettings;

        private readonly ILineService _lineService;
        private readonly IFigureIdService _figureIdService;
        private readonly IAuthorizationService _authorizationService;

        static LineOperations() {
            serializerSettings = new JsonSerializerSettings() {
                Converters = { new StringEnumConverter() }
            };
        }

        public LineOperations(ILineService lineService, IFigureIdService figureIdService, IAuthorizationService authorizationService) {
            _lineService = lineService;
            _figureIdService = figureIdService;
            _authorizationService = authorizationService;
        }

        public async Task CreateLine(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject jPayload) {
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(stringWebSocket.User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                return;//TODO REVER
            }

            InCreateWSLine inLine = jPayload.ToObject<InCreateWSLine>();

            if(inLine.BoardId != boardId) {
                return;//TODO REVER
            }

            var validationResults = new List<ValidationResult>();
            if(!Validator.TryValidateObject(inLine, new ValidationContext(inLine), validationResults, true)) {
                return;//TODO REVER
            }

            IFigureIdGenerator idGen = await _figureIdService.GetOrCreateFigureIdGeneratorAsync(boardId);
            long id = idGen.NewId();

            Line line = new Line { Id = id }.In(inLine);

            Task store = _lineService.CreateAsync(line, autoGenerateId: false);
            OperationUtils.ResolveTaskContinuation(store);

            Task response = stringWebSocket.SendAsync(
                new {
                    type = OperationType.CREATE_LINE,
                    payload = new { id = id, tempId = inLine.TempId }
                },
                serializerSettings
            );

            Task broadcast = session.BroadcastAsync(
                new {
                    type = OperationType.CREATE_LINE,
                    payload = new { figure = line.Out() }
                },
                serializerSettings
            );

            await Task.WhenAll(response, broadcast);
        }

        public async Task UpdateLine(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject jPayload) {
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(stringWebSocket.User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                return;//TODO REVER
            }

            InUpdateWSLine inLine = jPayload.ToObject<InUpdateWSLine>();

            if(inLine.BoardId != boardId) {
                return;//TODO REVER
            }

            var validationResults = new List<ValidationResult>();
            if(!Validator.TryValidateObject(inLine, new ValidationContext(inLine), validationResults, true)) {
                return;//TODO REVER
            }

            Line line = await _lineService.GetAsync(inLine.Id.Value, boardId);
            if(line == null) {
                return;//TODO REVER
            }

            line.In(inLine);

            Task store = _lineService.UpdateAsync(line);
            OperationUtils.ResolveTaskContinuation(store);

            await session.BroadcastAsync(
                new {
                    type = OperationType.ALTER_LINE,
                    payload = new {
                        offsetPoint = inLine.OffsetPoint,
                        figure = line.Out()
                    }
                },
                serializerSettings
            );
        }

        public async Task DeleteLine(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject jPayload) {
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(stringWebSocket.User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                return;//TODO REVER
            }

            if(!(jPayload.TryGetValue("id", System.StringComparison.OrdinalIgnoreCase, out JToken payload_id) && payload_id.Type == JTokenType.Integer)) {
                return;//TODO REVER
            }
            long id = payload_id.Value<long>();

            Line line = await _lineService.GetAsync(id, boardId);
            if(line == null) {
                return;//TODO REVER
            }

            Task store = _lineService.DeleteAsync(id, boardId);
            OperationUtils.ResolveTaskContinuation(store);

            await session.BroadcastAsync(
                new {
                    type = OperationType.DELETE_LINE,
                    payload = new { id = id }
                },
                serializerSettings
            );
        }
    }
}
