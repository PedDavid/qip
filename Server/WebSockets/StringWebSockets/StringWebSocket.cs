using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSockets.StringWebSockets {
    public class StringWebSocket : IDisposable {
        public WebSocket InternalWebSocket { get; }

        public StringWebSocket(WebSocket webSocket) {
            InternalWebSocket = webSocket;
        }

        public WebSocketCloseStatus? CloseStatus {
            get {
                return InternalWebSocket.CloseStatus;
            }
        }

        public string CloseStatusDescription {
            get {
                return InternalWebSocket.CloseStatusDescription;
            }
        }

        public WebSocketState State {
            get {
                return InternalWebSocket.State;
            }
        }

        public string SubProtocol {
            get {
                return InternalWebSocket.SubProtocol;
            }
        }

        public void Abort() {
            InternalWebSocket.Abort();
        }

        public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken) {
            return InternalWebSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);
        }

        public Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken) {
            return InternalWebSocket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);
        }

        public void Dispose() {
            InternalWebSocket.Dispose();
        }

        public async Task<string> ReceiveAsync() {
            WebSocketReceiveResult result;
            byte[] buffer = new byte[4 * 1024];
            StringBuilder builder = new StringBuilder();

            do {
                result = await InternalWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                builder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

            } while(!result.CloseStatus.HasValue && (!result.EndOfMessage || (result.EndOfMessage && builder.Length == 0)));

            return builder.ToString();
        }

        public async Task SendAsync(string message) {
            byte[] buffer = Encoding.UTF8.GetBytes(message);    // TODO(peddavid): Contract with webClient
            await InternalWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
