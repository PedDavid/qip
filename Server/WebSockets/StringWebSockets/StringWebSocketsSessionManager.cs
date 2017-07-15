using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

// TODO(peddavid): Change to more readable names

namespace WebSockets.StringWebSockets {
    public class StringWebSocketsSessionManager {
        private readonly ConcurrentDictionary<long, _StringWSSession> sessions 
            = new ConcurrentDictionary<long, _StringWSSession>();

        public IStringWebSocketSession Register(long roomId, StringWebSocket webSocket) {
            _StringWSSession psession = sessions.GetOrAdd(roomId, id => new _StringWSSession(id));
            return psession.Add(webSocket);
        }

        private class _StringWSSession {
            public long Id { get; }
            private readonly List<StringWebSocket> _wsl;
            private readonly ReaderWriterLockSlim _rwlock;

            public _StringWSSession(long id) {
                Id = id;
                _rwlock = new ReaderWriterLockSlim();
                _wsl = new List<StringWebSocket>();
            }

            public IStringWebSocketSession Add(StringWebSocket webSocket) {
                _rwlock.EnterWriteLock();
                try {
                    _wsl.Add(webSocket);

                    return new StringWSSession(webSocket, this);
                }
                finally {
                    _rwlock.ExitWriteLock();
                }
            }

            public void Remove(StringWebSocket webSocket) {
                _rwlock.EnterWriteLock();
                try {
                    _wsl.Remove(webSocket);
                }
                finally {
                    _rwlock.ExitWriteLock();
                }
            }

            public async Task BroadcastAsync(string message, Func<StringWebSocket, bool> filter) {
                _rwlock.EnterReadLock();
                try {
                    var sending = _wsl
                        .Where(ws => ws.State == WebSocketState.Open)
                        .Where(filter)
                        .Select(ws => ws.SendAsync(message))
                        .ToList();
                    await Task.WhenAll(sending);
                }
                finally {
                    _rwlock.ExitReadLock();
                }
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
