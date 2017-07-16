using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using WebSockets.Operations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebSockets.StringWebSockets {
    public class StringWebSocketsOperations {
        private readonly StringWebSocket _stringWebSocket;
        private readonly Dictionary<Models.Action, Operation> _operations;

        private readonly IStringWebSocketSession _session;
        private readonly long _roomId;

        public StringWebSocketsOperations(long roomId, StringWebSocket stringWebSocket, IStringWebSocketSession session, Dictionary<Models.Action, Operation> operations) { // TODO(peddavid): Change this "hard constructor"
            _roomId = roomId;
            _stringWebSocket = stringWebSocket;
            _session = session;
            _operations = operations;
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

                infoPayload["clientId"] = _roomId;

                OperationResult res = _operations[type]((JObject)infoPayload);

                await Task.WhenAll(
                    SendReponse(infoOwner, infoType, res.Response),
                    SendBroadcast(infoOwner, infoType, res.BroadcastMessage)
                );
                
            } while(!_stringWebSocket.CloseStatus.HasValue);

            _session.Exit();
            await _stringWebSocket.CloseAsync(_stringWebSocket.CloseStatus.Value, _stringWebSocket.CloseStatusDescription, CancellationToken.None);
        }

        // TODO(peddavid): WebSocket Response?
        private async Task SendReponse(JToken owner, JToken type, JToken response) {
            if(response != null) {
                dynamic res = new { owner = owner, type = type, payload = response };
                string jsonRes = await JsonConvert.SerializeObjectAsync(res);
                await _stringWebSocket.SendAsync(jsonRes);
            }
        }

        private async Task SendBroadcast(JToken owner, JToken type, JObject broadcast) {
            if(_session != null && broadcast != null) {
                dynamic res = new { owner = owner, type = type, payload = broadcast };
                string jsonRes = await JsonConvert.SerializeObjectAsync(res);
                await _session.BroadcastAsync(jsonRes);
            }
        }
    }
}
