using API.Domain;
using API.Interfaces;
using API.Interfaces.IServices;
using API.Interfaces.ServicesExceptions;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Output;
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
using WebSockets.Extensions;
using WebSockets.Models;
using WebSockets.StringWebSockets;

namespace WebSockets.Operations {
    public class LineOperations {
        private static readonly JsonSerializerSettings serializerSettings;

        private readonly ILineService _lineService;
        private readonly IFigureIdService _figureIdService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<LineOperations> _logger;

        static LineOperations() {
            serializerSettings = new JsonSerializerSettings() {
                Converters = { new StringEnumConverter() }
            };
        }

        public LineOperations(ILineService lineService, IFigureIdService figureIdService, IAuthorizationService authorizationService, ILogger<LineOperations> logger) {
            _lineService = lineService;
            _figureIdService = figureIdService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        public async Task CreateLine(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject jPayload) {
            ClaimsPrincipal user = stringWebSocket.User;
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(user, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.InsertWSLineNotAuthorized, "CreateLine (Board {boardId}) NOT AUTHORIZED {userId}", boardId, user.GetNameIdentifier());
                return;
            }

            InCreateWSLine inLine = jPayload.ToObject<InCreateWSLine>();

            var validationResults = new List<ValidationResult>();
            if(!Validator.TryValidateObject(inLine, new ValidationContext(inLine), validationResults, true)) {
                _logger.LogDebug(LoggingEvents.InsertWSLineInvalidModel, "CreateLine (Board {boardId}) INVALID MODEL", boardId);
                return;
            }

            if(inLine.BoardId != boardId) {
                _logger.LogDebug(LoggingEvents.InsertWSLineWrongBoardId, "CreateLine (Board {boardId}) WRONG BOARD ID {otherBoardId}", boardId, inLine.BoardId);
                return;
            }

            IFigureIdGenerator idGen = await _figureIdService.GetOrCreateFigureIdGeneratorAsync(boardId);

            Line line = new Line { Id = idGen.NewId() }.In(inLine);

            Task store = _lineService.CreateAsync(line, autoGenerateId: false);
            if(store.IsFaulted) {
                ReadOnlyCollection<Exception> exceptions = store.Exception.InnerExceptions;
                Exception serviceException = exceptions.First();
                if(exceptions.Count == 1 && serviceException.GetType() == typeof(ServiceException)) {
                    _logger.LogError(LoggingEvents.InsertWSLineUnexpectedServiceError, serviceException, "CreateLine (Board {boardId}) UNEXPECTED SERVICE ERROR", boardId);
                }
                else {
                    _logger.LogError(LoggingEvents.InsertWSLineUnexpectedError, store.Exception, "CreateLine (Board {boardId}) UNEXPECTED ERROR", boardId);
                }
                return;
            }

            Task messages = SendInsertMessages(stringWebSocket, session, line.Out(), inLine.TempId, inLine.PersistLocalBoard);
            try {
                await store;
                _logger.LogInformation(LoggingEvents.InsertWSLine, "Line {id} of Board {boardId} Created", line.Id, boardId);

                await messages;
            }
            catch(Exception e) {
                _logger.LogError(LoggingEvents.InsertWSLineUnexpectedError, e, "CreateLine (Board {boardId}) UNEXPECTED ERROR", boardId);
            }
        }

        private static Task SendInsertMessages(StringWebSocket stringWebSocket, IStringWebSocketSession session, OutLine line, long tempId, bool persistLocalBoard) {
            dynamic sendPayload;
            if(persistLocalBoard)
                sendPayload = new { figure = line };
            else
                sendPayload = new { id = line.Id, tempId = tempId };

            Task response = stringWebSocket.SendAsync(
                new {
                    type = OperationType.CREATE_LINE,
                    payload = sendPayload
                },
                serializerSettings
            );

            Task broadcast = session.BroadcastAsync(
                new {
                    type = OperationType.CREATE_LINE,
                    payload = new { figure = line }
                },
                serializerSettings
            );

            return Task.WhenAll(response, broadcast);
        }

        public async Task UpdateLine(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject jPayload) {
            ClaimsPrincipal user = stringWebSocket.User;
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(user, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.UpdateWSLineNotAuthorized, "UpdateLine (Board {boardId}) NOT AUTHORIZED {userId}", boardId, user.GetNameIdentifier());
                return;
            }

            InUpdateWSLine inLine = jPayload.ToObject<InUpdateWSLine>();

            var validationResults = new List<ValidationResult>();
            if(!Validator.TryValidateObject(inLine, new ValidationContext(inLine), validationResults, true)) {
                _logger.LogDebug(LoggingEvents.UpdateWSLineInvalidModel, "UpdateLine (Board {boardId}) INVALID MODEL", boardId);
                return;
            }

            if(inLine.BoardId != boardId) {
                _logger.LogDebug(LoggingEvents.UpdateWSLineWrongBoardId, "UpdateLine (Board {boardId}) WRONG BOARD ID {otherBoardId}", boardId, inLine.BoardId);
                return;
            }
      
            Line line = await _lineService.GetAsync(inLine.Id.Value, boardId);
            if(line == null) {
                _logger.LogWarning(LoggingEvents.UpdateWSLineNotFound, "UpdateLine {id} (Board {boardId}) NOT FOUND", line.Id, boardId);
                return;
            }

            line.In(inLine);

            Task store = _lineService.UpdateAsync(line);
            if(store.IsFaulted) {
                ReadOnlyCollection<Exception> exceptions = store.Exception.InnerExceptions;
                Exception serviceException = exceptions.First();
                if(exceptions.Count == 1 && serviceException.GetType() == typeof(ServiceException)) {
                    _logger.LogError(LoggingEvents.UpdateWSLineUnexpectedServiceError, serviceException, "UpdateLine {id} (Board {boardId}) UNEXPECTED SERVICE ERROR", line.Id, boardId);
                }
                else {
                    _logger.LogError(LoggingEvents.UpdateWSLineUnexpectedError, store.Exception, "UpdateLine {id} (Board {boardId}) UNEXPECTED ERROR", line.Id, boardId);
                }
                return;
            }
 
            Task messages = SendUpdateMessages(session, line.Out(), inLine.OffsetPoint, inLine.IsScaling);
            try {
                await store;
                _logger.LogInformation(LoggingEvents.UpdateWSLine, "Line {id} of Board {boardId} Updated", line.Id, boardId);

                await messages;
            }
            catch(Exception e) {
                _logger.LogError(LoggingEvents.UpdateWSLineUnexpectedError, e, "UpdateLine {id} (Board {boardId}) UNEXPECTED ERROR", line.Id, boardId);
            }
        }
        private static Task SendUpdateMessages(IStringWebSocketSession session, OutLine line, Offset offsetPoint, string isScaling) {
            return session.BroadcastAsync(
                new {
                    type = OperationType.ALTER_LINE,
                    payload = new {
                        offsetPoint = offsetPoint,
                        isScaling = isScaling,
                        figure = line
                    }
                },
                serializerSettings
            );
        }

        public async Task DeleteLine(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject jPayload) {
            ClaimsPrincipal user = stringWebSocket.User;
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(user, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.DeleteWSLineNotAuthorized, "DeleteLine (Board {boardId}) NOT AUTHORIZED {userId}", boardId, user.GetNameIdentifier());
                return;
            }

            if(!(jPayload.TryGetValue("id", System.StringComparison.OrdinalIgnoreCase, out JToken payload_id) && payload_id.Type == JTokenType.Integer)) {
                _logger.LogDebug(LoggingEvents.DeleteWSLineInvalidModel, "DeleteLine (Board {boardId}) INVALID MODEL", boardId);
                return;
            }
            long id = payload_id.Value<long>();

            Line line = await _lineService.GetAsync(id, boardId);
            if(line == null) {
                _logger.LogWarning(LoggingEvents.DeleteWSLineNotFound, "DeleteLine {id} (Board {boardId}) NOT FOUND", line.Id, boardId);
                return;
            }

            Task store = _lineService.DeleteAsync(id, boardId);
            if(store.IsFaulted) {
                ReadOnlyCollection<Exception> exceptions = store.Exception.InnerExceptions;
                Exception serviceException = exceptions.First();
                if(exceptions.Count == 1 && serviceException.GetType() == typeof(ServiceException)) {
                    _logger.LogError(LoggingEvents.DeleteWSLineUnexpectedServiceError, serviceException, "DeleteLine {id} (Board {boardId}) UNEXPECTED SERVICE ERROR", line.Id, boardId);
                }
                else {
                    _logger.LogError(LoggingEvents.DeleteWSLineUnexpectedError, store.Exception, "DeleteLine {id} (Board {boardId}) UNEXPECTED ERROR", line.Id, boardId);
                }
                return;
            }

            Task messages = SendDeleteMessages(session, id);
            try {
                await store;
                _logger.LogInformation(LoggingEvents.InsertWSLine, "Line {id} of Board {boardId} Deleted", line.Id, boardId);

                await messages;
            }
            catch(Exception e) {
                _logger.LogError(LoggingEvents.InsertWSLineUnexpectedError, e, "DeleteLine {id} (Board {boardId}) UNEXPECTED ERROR", line.Id, boardId);
            }
        }

        private static Task SendDeleteMessages(IStringWebSocketSession session, long id) {
            return session.BroadcastAsync(
                new {
                    type = OperationType.DELETE_LINE,
                    payload = new { id = id }
                },
                serializerSettings
            );
        }
    }
}
