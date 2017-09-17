using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using QIP.WebSockets.Models;
using QIP.WebSockets.Operations;

namespace QIP.WebSockets.StringWebSockets {
    public class StringWebSocketsOperations {
        private readonly StringWebSocket _stringWebSocket;
        private readonly Dictionary<Models.OperationType, Operation> _operations;

        private readonly IStringWebSocketSession _session;
        private readonly long _roomId;

        public StringWebSocketsOperations(long roomId, StringWebSocket stringWebSocket, IStringWebSocketSession session, Dictionary<Models.OperationType, Operation> operations) { // TODO(peddavid): Change this "hard constructor"
            _roomId = roomId;
            _stringWebSocket = stringWebSocket;
            _session = session;
            _operations = operations;
        }

        public async Task AcceptRequests() {
            try {
                do {
                    string msg = await _stringWebSocket.ReceiveAsync();

                    if(string.IsNullOrWhiteSpace(msg)) {
                        continue;//TODO REVER
                    }

                    WSMessage info = JsonConvert.DeserializeObject<WSMessage>(msg);

                    var validationResults = new List<ValidationResult>();
                    if(!Validator.TryValidateObject(info, new ValidationContext(info), validationResults, true)) {
                        continue;//TODO REVER
                    }

                    OperationType type = info.Type.Value;

                    await _operations[type](_stringWebSocket, _session, info.Payload);

                } while(!_stringWebSocket.CloseStatus.HasValue);
            }
            catch(WebSocketException e) {
                ////TODO REVER
            }
            finally {
                _session.Exit();
                await _stringWebSocket.CloseAsync(_stringWebSocket.CloseStatus.Value, _stringWebSocket.CloseStatusDescription, CancellationToken.None);
            }
        }
    }
}
