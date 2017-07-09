using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebSockets.StringWebSockets {
    public class StringWebSocketsSessionManager {
        private readonly ConcurrentDictionary<long, _StringWSSession> sessions;

        public StringWebSocketsSessionManager() {
            sessions = new ConcurrentDictionary<long, _StringWSSession>();
        }

        public IStringWebSocketSession Register(long sessionId, StringWebSocket webSocket) {
            _StringWSSession psession = sessions.GetOrAdd(sessionId, id => new _StringWSSession(id));
            return psession.Add(webSocket);
        }

        private class _StringWSSession {
            public long Id { get; }
            private readonly List<StringWebSocket> items;
            private readonly ReaderWriterLockSlim _rwlock;

            public _StringWSSession(long id) {
                Id = id;
                _rwlock = new ReaderWriterLockSlim();
                items = new List<StringWebSocket>();
            }

            public IStringWebSocketSession Add(StringWebSocket webSocket) {
                _rwlock.EnterWriteLock();
                try {
                    items.Add(webSocket);

                    return new StringWSSession(webSocket, this);
                }
                finally {
                    _rwlock.ExitWriteLock();
                }
            }

            public void Remove(StringWebSocket webSocket) {
                _rwlock.EnterWriteLock();
                try {
                    items.Remove(webSocket);
                }
                finally {
                    _rwlock.ExitWriteLock();
                }
            }

            public async Task BroadcastAsync(string message, Func<StringWebSocket, bool> filter) {
                List<Task> sending = new List<Task>();

                _rwlock.EnterReadLock();
                try {
                    items
                        .Where(ws => ws.State == WebSocketState.Open)
                        .Where(filter)
                        .Select(ws => ws.SendAsync(message))
                        .Aggregate(sending, (s, t) => { s.Add(t); return s; });
                }
                finally {
                    _rwlock.ExitReadLock();
                }

                await Task.WhenAll(sending);
            }
        }

        private class StringWSSession : IStringWebSocketSession {
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

            public Task BroadcastAsync(string message) {
                return _session.BroadcastAsync(message, ws => ws != _webSocket);
            }
        }
    }
}
