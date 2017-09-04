using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Threading.Tasks;
using WebSockets.StringWebSockets;

namespace WebSockets.Extensions
{
    public static class WebSocketManagerExtensions
    {
        public static async Task<StringWebSocket> AcceptStringWebSocketAsync(this WebSocketManager webSocketManager) {
            WebSocket webSocket = await webSocketManager.AcceptWebSocketAsync();

            return new StringWebSocket(webSocket);
        }

        public static async Task<StringWebSocket> AcceptStringWebSocketAsync(this WebSocketManager webSocketManager, ClaimsPrincipal user) {
            WebSocket webSocket = await webSocketManager.AcceptWebSocketAsync();

            return new StringWebSocket(webSocket, user);
        }
    }
}
