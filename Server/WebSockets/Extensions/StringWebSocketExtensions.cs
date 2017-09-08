using Newtonsoft.Json;
using System.Threading.Tasks;
using WebSockets.StringWebSockets;

namespace WebSockets.Extensions {
    public static class StringWebSocketExtensions {
        public static Task SendAsync(this StringWebSocket webSocket, object message, JsonSerializerSettings serializerSettings = null) {
            return webSocket.SendAsync(JsonConvert.SerializeObject(message, serializerSettings));
        }
    }
}
