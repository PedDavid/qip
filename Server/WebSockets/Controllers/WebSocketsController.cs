using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using WebSockets.Extensions;
using WebSockets.StringWebSockets;
using WebSockets.Operations;

using API.Interfaces.IRepositories;
using API.Services;

namespace WebSockets.Controllers {
    [Route("api/[controller]")] // TODO(peddavid): Another Route "/ws"?
    public class WebSocketsController : Controller {
        private StringWebSocketsSessionManager _sessionManager;
        private readonly LineOperations _lineOperations;
        private readonly ImageOperations _imageOperations;

        private readonly Dictionary<Models.Action, Operation> _operations;  // TODO(peddavid): Should this be immutable?

        public WebSocketsController(StringWebSocketsSessionManager sessionManager, IFigureIdRepository figureIdRepository, IImageRepository imageRepository, ILineRepository lineRepository) {
            _sessionManager = sessionManager;
            var idGen = FigureIdGenerator.Create(figureIdRepository);
            _imageOperations = new ImageOperations(imageRepository, idGen);
            _lineOperations = new LineOperations(lineRepository, idGen);

            _operations = new Dictionary<Models.Action, Operation>() {
                { Models.Action.CREATE_IMAGE, _imageOperations.CreateImage },
                { Models.Action.DELETE_IMAGE, _imageOperations.DeleteImage },
                { Models.Action.ALTER_IMAGE, _imageOperations.UpdateImage },
                { Models.Action.CREATE_LINE, _lineOperations.CreateLine },
                { Models.Action.DELETE_LINE, _lineOperations.DeleteLine },
                { Models.Action.ALTER_LINE, _lineOperations.UpdateLine }
            };
        }

        [HttpGet]
        public async Task Index(long? id) {
            if(HttpContext.WebSockets.IsWebSocketRequest && id.HasValue) {
                StringWebSocket webSocket = await HttpContext.WebSockets.AcceptStringWebSocketAsync();

                long clientId = id.Value; // TODO(peddavid): Rename?
                var session = _sessionManager.Register(clientId, webSocket);
                var swsopers = new StringWebSocketsOperations(clientId, webSocket, session, _operations);

                await swsopers.AcceptRequests();
            }
            else {
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
