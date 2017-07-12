using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using WebSockets.Models;
using WebSockets.Operations;

namespace WebSockets.StringWebSockets {
    public class StringWebSocketsOperations {
        private readonly StringWebSocket _stringWebSocket;
        private readonly Dictionary<Models.Action, Operation> _operations;

        public IStringWebSocketSession Session { get; set; }

        public long? ClientId { get; set; }

        public StringWebSocketsOperations(StringWebSocket stringWebSocket, Dictionary<Models.Action, Operation> operations) {
            _stringWebSocket = stringWebSocket;
            _operations = operations;
        }

        public StringWebSocketsOperations(StringWebSocket stringWebSocket) : this(stringWebSocket, new Dictionary<Models.Action, Operation>()) {}

        public StringWebSocketsOperations(WebSocket webSocket) : this(new StringWebSocket(webSocket)) {}

        public StringWebSocketsOperations AddOperation(Models.Action action, Operation operation) {
            _operations.Add(action, operation);
            return this;
        }

        public void RemoveOperation(Models.Action action) {
            _operations.Remove(action);
        }

        public async Task AcceptRequests() {
            do {
                string msg = await _stringWebSocket.ReceiveAsync();

                JObject info = JObject.Parse(msg);

                if(!(info.TryGetValue("owner", StringComparison.OrdinalIgnoreCase, out JToken infoOwner) && infoOwner.Type == JTokenType.Integer)) {

                    continue;//TODO REVER
                }

                if(!(info.TryGetValue("type", StringComparison.OrdinalIgnoreCase, out JToken infoType) && infoType.Type == JTokenType.String)) {

                    continue;//TODO REVER
                }

                if(!Enum.TryParse(infoType.Value<string>(), true, out Models.Action type)) {

                    continue;//TODO REVER
                }

                if(!(info.TryGetValue("payload", StringComparison.OrdinalIgnoreCase, out JToken infoPayload) && infoPayload.Type == JTokenType.Object)) {

                    continue;//TODO REVER
                }

                if(ClientId.HasValue)
                    infoPayload["clientId"] = ClientId.Value;

                OperationResult res = _operations[type]((JObject)infoPayload);

                await Task.WhenAll(
                    SendReponse(infoOwner, infoType, res.Response),
                    SendBroadcast(infoOwner, infoType, res.BroadcastMessage)
                );
                
            } while(!_stringWebSocket.CloseStatus.HasValue);

            Session.Exit();
            await _stringWebSocket.CloseAsync(_stringWebSocket.CloseStatus.Value, _stringWebSocket.CloseStatusDescription, CancellationToken.None);
        }

        private async Task SendReponse(JToken owner, JToken type, JToken response) {
            if(response != null) {
                dynamic res = new { owner = owner, type = type, payload = response };
                string jsonRes = await JsonConvert.SerializeObjectAsync(res);
                await _stringWebSocket.SendAsync(jsonRes);
            }
        }

        private async Task SendBroadcast(JToken owner, JToken type, JObject broadcast) {
            if(Session != null && broadcast != null) {
                dynamic res = new { owner = owner, type = type, payload = broadcast };
                string jsonRes = await JsonConvert.SerializeObjectAsync(res);
                await Session.BroadcastAsync(jsonRes);
            }
        }
    }
}
