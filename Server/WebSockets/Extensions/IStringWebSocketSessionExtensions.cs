using Newtonsoft.Json;
using System.Threading.Tasks;
using WebSockets.StringWebSockets;

namespace WebSockets.Extensions {
    public static class IStringWebSocketSessionExtensions {
        public static Task BroadcastAsync(this IStringWebSocketSession session, object message, JsonSerializerSettings serializerSettings = null) {
            return session.BroadcastAsync(JsonConvert.SerializeObject(message, serializerSettings));
        }
    }
}
