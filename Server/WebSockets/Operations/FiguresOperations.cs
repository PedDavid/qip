using QIP.Public;
using QIP.Public.IServices;
using QIP.Public.ServicesExceptions;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
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
using System.Text;
using System.Threading.Tasks;
using QIP.WebSockets.Extensions;
using QIP.WebSockets.Models;
using QIP.WebSockets.StringWebSockets;

namespace QIP.WebSockets.Operations {
    class FiguresOperations {
        private static readonly JsonSerializerSettings serializerSettings;

        private readonly IFiguresService _figuresService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<FiguresOperations> _logger;

        static FiguresOperations() {
            serializerSettings = new JsonSerializerSettings() {
                Converters = { new StringEnumConverter() }
            };
        }

        public FiguresOperations(IFiguresService figuresService,IAuthorizationService authorizationService, ILogger<FiguresOperations> logger) {
            _figuresService = figuresService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        public async Task DeleteFigures(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject payload) {
            ClaimsPrincipal user = stringWebSocket.User;
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(user, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(
                    LoggingEvents.DeleteWSFiguresNotAuthorized, 
                    "DeleteFigures (Board {boardId}) NOT AUTHORIZED {userId}", 
                    boardId, 
                    user.GetNameIdentifier()
                );
                return;
            }

            DeleteWSFigures delFig = payload.ToObject<DeleteWSFigures>();

            var validationResults = new List<ValidationResult>();
            if(!Validator.TryValidateObject(delFig, new ValidationContext(delFig), validationResults, true)) {
                _logger.LogDebug(LoggingEvents.DeleteWSFiguresInvalidModel, "DeleteFigures (Board {boardId}) INVALID MODEL", boardId);
                return;
            }

            if(delFig.BoardId != boardId) {
                _logger.LogDebug(LoggingEvents.DeleteWSFiguresWrongBoardId, "DeleteFigures (Board {boardId}) WRONG BOARD ID {otherBoardId}", boardId, delFig.BoardId);
                return;
            }

            Task store = _figuresService.DeleteAsync(boardId, delFig.MaxFigureId);
            if(store.IsFaulted) {
                LogSynchronousFault(store, _logger, boardId, delFig.MaxFigureId);
                return;
            }

            Task messages = SendDeleteMessages(session, delFig);
            try {
                await store;
                _logger.LogInformation(
                    LoggingEvents.DeleteWSFigures, 
                    "Figures up to {lastFigureToDelete} of Board {boardId} Deleted", 
                    delFig.MaxFigureId, 
                    boardId
                );

                await messages;
            }
            catch(Exception e) {
                _logger.LogError(
                    LoggingEvents.DeleteWSFiguresUnexpectedError, 
                    e, 
                    "DeleteFigures up to {lastFigureToDelete} (Board {boardId}) UNEXPECTED ERROR", 
                    delFig.MaxFigureId, 
                    boardId
                );
            }
        }

        private static void LogSynchronousFault(Task store, ILogger<FiguresOperations> logger, long boardId, long lastFigureToDelete) {
            ReadOnlyCollection<Exception> exceptions = store.Exception.InnerExceptions;
            Exception serviceException = exceptions.First();
            if(exceptions.Count == 1 && serviceException.GetType() == typeof(ServiceException)) {
                logger.LogError(
                    LoggingEvents.DeleteWSFiguresUnexpectedServiceError,
                    serviceException,
                    "DeleteFigures up to {lastFigureToDelete} (Board {boardId}) UNEXPECTED SERVICE ERROR",
                    lastFigureToDelete,
                    boardId
                );
            }
            else {
                logger.LogError(
                    LoggingEvents.DeleteWSFiguresUnexpectedError,
                    store.Exception,
                    "DeleteFigures up to {lastFigureToDelete} (Board {boardId}) UNEXPECTED ERROR",
                    lastFigureToDelete,
                    boardId
                );
            }
        }

        private static Task SendDeleteMessages(IStringWebSocketSession session, DeleteWSFigures delFig) {
            return session.BroadcastAsync(
                new {
                    type = OperationType.BOARD_CLEAN,
                    payload = delFig
                },
                serializerSettings
            );
        }
    }
}
