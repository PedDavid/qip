using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;

namespace API.WebSockets {
    public interface IStringWSSession {
        long Id { get; }

        Task BroadcastAsync(string message);

        void Exit();
    }
}
