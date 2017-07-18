using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebSockets.StringWebSockets;

namespace WebSockets.Operations
{
    public delegate Task Operation(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject payload);
}
