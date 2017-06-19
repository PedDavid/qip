using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace API.WebSockets {
    public static class StringWebSocketExtensions {
        public static async Task<StringWebSocket> AcceptStringWebSocketAsync(this WebSocketManager webSocketManager) {
            WebSocket webSocket = await webSocketManager.AcceptWebSocketAsync();

            return new StringWebSocket(webSocket);
        }
    }
}
