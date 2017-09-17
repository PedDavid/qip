using Newtonsoft.Json;
using System.Threading.Tasks;
using QIP.WebSockets.StringWebSockets;

namespace QIP.WebSockets.Extensions {
    public static class StringWebSocketExtensions {
        public static Task SendAsync(this StringWebSocket webSocket, object message, JsonSerializerSettings serializerSettings = null) {
            return webSocket.SendAsync(JsonConvert.SerializeObject(message, serializerSettings));
        }
    }
}
