using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QIP.WebSockets.StringWebSockets {
    public interface IStringWebSocketSession {
        long Id { get; }

        Task BroadcastAsync(string message);

        void Exit();
    }
}
