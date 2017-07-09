using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebSockets.Extensions;
using WebSockets.StringWebSockets;
using API.Interfaces.IRepositories;
using WebSockets.Operations;

namespace WebSockets.Controllers {
    [Route("api/[controller]")]
    public class WebSocketsController : Controller {
        private StringWebSocketsSessionManager _sessionManager;
        private readonly LineOperations _lineOperations;
        private readonly ImageOperations _imageOperations;

        public WebSocketsController(StringWebSocketsSessionManager sessionManager, IImageRepository imageRepository, ILineRepository lineRepository) {
            _sessionManager = sessionManager;
            _imageOperations = new ImageOperations(imageRepository);
            _lineOperations = new LineOperations(lineRepository);
        }

        [HttpGet]
        public async Task Index(long? id) {
            if(HttpContext.WebSockets.IsWebSocketRequest && id.HasValue) {
                StringWebSocket webSocket = await HttpContext.WebSockets.AcceptStringWebSocketAsync();

                var swsopers = new StringWebSocketsOperations(webSocket) {
                    Session = _sessionManager.Register(id.Value, webSocket)
                };

                LoadActionOperations(swsopers);

                await swsopers.AcceptRequests();
            }
            else {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private void LoadActionOperations(StringWebSocketsOperations operations) {
            operations.AddOperation(Models.Action.CREATE_IMAGE, _imageOperations.CreateImage);
            //TODO
        }
    }
}
