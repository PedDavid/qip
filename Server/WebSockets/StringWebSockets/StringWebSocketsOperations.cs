using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using WebSockets.Models;
using WebSockets.Operations;

namespace WebSockets.StringWebSockets {
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
            do {
                string msg = await _stringWebSocket.ReceiveAsync();

                if(string.IsNullOrWhiteSpace(msg)) {

                    continue;//TODO REVER
                }

                WSMessage info = JsonConvert.DeserializeObject<WSMessage>(msg);

                var validationResults = new List<ValidationResult>();
                if(!Validator.TryValidateObject(info, new ValidationContext(info), validationResults, true)) {
                    return;//TODO REVER
                }

                OperationType type = info.Type.Value;

                await _operations[type](_stringWebSocket, _session, info.Payload);

            } while(!_stringWebSocket.CloseStatus.HasValue);

            _session.Exit();
            await _stringWebSocket.CloseAsync(_stringWebSocket.CloseStatus.Value, _stringWebSocket.CloseStatusDescription, CancellationToken.None);
        }
    }
}
