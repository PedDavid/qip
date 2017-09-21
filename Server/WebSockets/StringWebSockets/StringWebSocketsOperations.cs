using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using QIP.WebSockets.Models;
using QIP.WebSockets.Operations;
using Microsoft.Extensions.Logging;
using QIP.Public;

namespace QIP.WebSockets.StringWebSockets {
    public class StringWebSocketsOperations {
        private readonly StringWebSocket _stringWebSocket;
        private readonly Dictionary<OperationType, Operation> _operations;
        private readonly ILogger<StringWebSocketsOperations> _logger;
        private readonly IStringWebSocketSession _session;
        private readonly long _roomId;

        public StringWebSocketsOperations(long roomId, StringWebSocket stringWebSocket, IStringWebSocketSession session, Dictionary<OperationType, Operation> operations, ILogger<StringWebSocketsOperations> logger) { // TODO(peddavid): Change this "hard constructor"
            _roomId = roomId;
            _stringWebSocket = stringWebSocket;
            _session = session;
            _operations = operations;
            _logger = logger;
        }

        public async Task AcceptRequests() {
            try {
                do {
                    try {
                        string msg = await _stringWebSocket.ReceiveAsync();

                        if(string.IsNullOrWhiteSpace(msg)) {
                            _logger.LogDebug(LoggingEvents.WSOperationsInvalidMessage, "WebSocket Operations (Room {roomId}) INVALID MESSAGE", _roomId);
                            continue;
                        }

                        WSMessage info = JsonConvert.DeserializeObject<WSMessage>(msg);

                        var validationResults = new List<ValidationResult>();
                        if(!Validator.TryValidateObject(info, new ValidationContext(info), validationResults, true)) {
                            _logger.LogDebug(LoggingEvents.WSOperationsInvalidModel, "WebSocket Operations (Room {roomId}) INVALID MODEL", _roomId);
                            continue;
                        }

                        OperationType type = info.Type.Value;

                        await _operations[type](_stringWebSocket, _session, info.Payload);
                    }
                    catch(WebSocketException e) {
                        _logger.LogInformation(LoggingEvents.WSOperationsEndedAbruptly, e, "WebSocket Operations (Room {roomId}) USER ABRUPTLY ENDED", _roomId);
                        return;
                    }
                    catch(Exception e) {
                        _logger.LogCritical(LoggingEvents.WSOperationsUnhandledException, e, "WebSocket Operations (Room {roomId}) UNHANDLED EXCEPTION", _roomId);
                    }
                } while(!_stringWebSocket.CloseStatus.HasValue);
            }
            finally {
                _session.Exit();
                await _stringWebSocket.CloseAsync(_stringWebSocket.CloseStatus.Value, _stringWebSocket.CloseStatusDescription, CancellationToken.None);
            }
        }
    }
}
