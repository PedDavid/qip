using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.WebSockets;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class WebSocketsController : Controller
    {
        private StringWSSessionsManager _ws;
        private Dictionary<WebSockets.Action, Func<dynamic, Task<int>>> _operations;

        public WebSocketsController(StringWSSessionsManager ws, WSBoardOperationsFactory of) {
            _ws = ws;
            _operations = of.Generate();
        }

        [HttpGet]
        public async Task Index(long? id) {
            if(HttpContext.WebSockets.IsWebSocketRequest && id.HasValue) {
                StringWebSocket webSocket = await HttpContext.WebSockets.AcceptStringWebSocketAsync();

                //await Echo(context, webSocket);
                var session = _ws.Register(id.Value, HttpContext, webSocket);
                await new WSBoardOperations(HttpContext, webSocket, session, _operations).Start();
            }
            else {
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
