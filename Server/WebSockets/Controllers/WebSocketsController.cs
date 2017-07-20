using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using WebSockets.Extensions;
using WebSockets.StringWebSockets;
using WebSockets.Operations;

using API.Interfaces.IRepositories;
using API.Services;
using Microsoft.Extensions.Caching.Memory;

namespace WebSockets.Controllers {
    [Route("ws")]
    public class WebSocketsController : Controller {
        private readonly IBoardRepository _boardRepository;
        private readonly StringWebSocketsSessionManager _sessionManager;
        private readonly LineOperations _lineOperations;
        private readonly ImageOperations _imageOperations;

        private readonly Dictionary<Models.Action, Operation> _operations;  // TODO(peddavid): Should this be immutable?

        public WebSocketsController(
            IBoardRepository boardRepository,
            StringWebSocketsSessionManager sessionManager, 
            IFigureIdRepository figureIdRepository, 
            IImageRepository imageRepository, 
            ILineRepository lineRepository, 
            IMemoryCache memoryCache
        ) {
            _boardRepository = boardRepository;
            _sessionManager = sessionManager;
            _imageOperations = new ImageOperations(imageRepository, memoryCache, figureIdRepository);
            _lineOperations = new LineOperations(lineRepository, memoryCache, figureIdRepository);

            _operations = new Dictionary<Models.Action, Operation>() {
                { Models.Action.CREATE_IMAGE, _imageOperations.CreateImage },
                { Models.Action.DELETE_IMAGE, _imageOperations.DeleteImage },
                { Models.Action.ALTER_IMAGE, _imageOperations.UpdateImage },
                { Models.Action.CREATE_LINE, _lineOperations.CreateLine },
                { Models.Action.DELETE_LINE, _lineOperations.DeleteLine },
                { Models.Action.ALTER_LINE, _lineOperations.UpdateLine }
            };
        }

        [HttpGet("{roomId}")]
        public async Task Index(long roomId) {
            if(HttpContext.WebSockets.IsWebSocketRequest && await _boardRepository.ExistsAsync(roomId)) {
                StringWebSocket webSocket = await HttpContext.WebSockets.AcceptStringWebSocketAsync();

                var session = _sessionManager.Register(roomId, webSocket);
                var swsopers = new StringWebSocketsOperations(roomId, webSocket, session, _operations);

                await swsopers.AcceptRequests();
            }
            else {
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
