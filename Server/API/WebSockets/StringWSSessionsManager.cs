using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace API.WebSockets {
    public class StringWSSessionsManager {
        private readonly ConcurrentDictionary<long, _StringWSSession> sessions;

        public StringWSSessionsManager() {
            sessions = new ConcurrentDictionary<long, _StringWSSession>();
        }

        //TODO Ver se HttpContext é preciso
        public IStringWSSession Register(long sessionId, HttpContext context, StringWebSocket webSocket) {
            _StringWSSession psession = sessions.GetOrAdd(sessionId, new _StringWSSession(sessionId));
            return psession.Add(webSocket);
        }

        private class _StringWSSession {
            public long Id { get; }
            private readonly ConcurrentQueue<StringWebSocket> items;
            private readonly object _lockObj;

            public _StringWSSession(long id) {
                Id = id;
                items = new ConcurrentQueue<StringWebSocket>();
            }

            public IStringWSSession Add(StringWebSocket webSocket) {
                items.Enqueue(webSocket);

                return new StringWSSession(webSocket, this);
            }

            private void Remove(StringWebSocket webSocket) {
                //TODO Sugerir não fazer e filtrar por estado no broadcast
            }

            private class StringWSSession : IStringWSSession {
                private readonly StringWebSocket _webSocket;
                private readonly _StringWSSession _session;

                public long Id {
                    get {
                        return _session.Id;
                    }
                }

                public StringWSSession(StringWebSocket webSocket, _StringWSSession session) {
                    _webSocket = webSocket;
                    _session = session;
                }

                public void Exit() {
                    _session.Remove(_webSocket);
                }

                public async Task BroadcastAsync(string message) {
                    foreach(StringWebSocket ws in _session.items.Where(ws => ws != _webSocket && ws.State == WebSocketState.Open)) {
                        await ws.SendAsync(message);
                    }
                }
            }
        }
    }
}
