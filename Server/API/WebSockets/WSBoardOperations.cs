using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace API.WebSockets {
    public class WSBoardOperations {
        private readonly IStringWSSession _session;
        private readonly HttpContext _context;
        private readonly StringWebSocket _stringWebSocket;

        private readonly Dictionary<Action, Func<dynamic, Task<int>>> _operations;

        public WSBoardOperations(HttpContext context, StringWebSocket stringWebSocket, IStringWSSession session, Dictionary<Action, Func<dynamic, Task<int>>> operations) {
            _session = session;
            _context = context;
            _stringWebSocket = stringWebSocket;
            _operations = operations;
        }

        public WSBoardOperations(
            HttpContext context,
            WebSocket webSocket,
            IStringWSSession session,
            Dictionary<Action, Func<dynamic, Task<int>>> operations
            ) : this(context, new StringWebSocket(webSocket), session, operations) { }

        public async Task Start() {
            do {
                string msg = await _stringWebSocket.ReceiveAsync();

                if(String.IsNullOrWhiteSpace(msg))
                    continue;//TODO REVER

                dynamic info = JObject.Parse(msg);
                dynamic infoType = info["type"];

                if(infoType == null)
                    continue;//TODO REVER

                if(!Enum.TryParse(infoType.Value as string, true, out Action type))
                    continue;//TODO REVER

                dynamic infoPayload = info["payload"];

                if(infoPayload == null)
                    continue;//TODO REVER

                //dynamic res = await _operations[type](infoPayload);

                //TODO Gerar o id
                //await _stringWebSocket.SendAsync(id.toString());

                await _session.BroadcastAsync(msg);
            } while(!_stringWebSocket.CloseStatus.HasValue);

            _session.Exit();
            await _stringWebSocket.CloseAsync(_stringWebSocket.CloseStatus.Value, _stringWebSocket.CloseStatusDescription, CancellationToken.None);
        }
    }
}
