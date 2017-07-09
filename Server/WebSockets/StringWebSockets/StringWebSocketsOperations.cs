using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using WebSockets.Models;

namespace WebSockets.StringWebSockets {
    public class StringWebSocketsOperations {
        private readonly StringWebSocket _stringWebSocket;
        private readonly Dictionary<Models.Action, Func<JToken, Task<dynamic>>> _operations;

        public IStringWebSocketSession Session { get; set; }

        public StringWebSocketsOperations(StringWebSocket stringWebSocket) {
            _stringWebSocket = stringWebSocket;
            _operations = new Dictionary<Models.Action, Func<JToken, Task<dynamic>>>();
        }

        public StringWebSocketsOperations(WebSocket webSocket) : this(new StringWebSocket(webSocket)) {}

        public StringWebSocketsOperations AddOperation(Models.Action action, Func<JToken, Task<dynamic>> operation) {
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

                if(!(info.TryGetValue("type", StringComparison.OrdinalIgnoreCase, out JToken infoType) && infoType.Type == JTokenType.String))
                    continue;//TODO REVER


                if(!Enum.TryParse(infoType.Value<string>(), true, out Models.Action type))
                    continue;//TODO REVER

                if(!(info.TryGetValue("payload", StringComparison.OrdinalIgnoreCase, out JToken infoPayload) && infoPayload.Type == JTokenType.Object))
                    continue;//TODO REVER

                //TODO Gerar o id
                dynamic res = await _operations[type](infoPayload);

                string jsonRes = await JsonConvert.SerializeObjectAsync(res);

                await _stringWebSocket.SendAsync(jsonRes);

                await Session.BroadcastAsync(msg);
            } while(!_stringWebSocket.CloseStatus.HasValue);

            Session.Exit();
            await _stringWebSocket.CloseAsync(_stringWebSocket.CloseStatus.Value, _stringWebSocket.CloseStatusDescription, CancellationToken.None);
        }
    }
}
