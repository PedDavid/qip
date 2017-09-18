using Newtonsoft.Json;
using System.Threading.Tasks;
using QIP.WebSockets.StringWebSockets;

namespace QIP.WebSockets.Extensions {
    public static class IStringWebSocketSessionExtensions {
        public static Task BroadcastAsync(this IStringWebSocketSession session, object message, JsonSerializerSettings serializerSettings = null) {
            return session.BroadcastAsync(JsonConvert.SerializeObject(message, serializerSettings));
        }
    }
}
