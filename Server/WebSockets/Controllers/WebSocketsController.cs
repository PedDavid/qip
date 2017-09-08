using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSockets.Extensions;
using WebSockets.Operations;
using WebSockets.StringWebSockets;

namespace WebSockets.Controllers {
    [Route("ws")]
    public class WebSocketsController : Controller {
        private readonly IBoardService _boardService;
        private readonly StringWebSocketsSessionManager _sessionManager;
        private readonly LineOperations _lineOperations;
        private readonly ImageOperations _imageOperations;
        private readonly IAuthorizationService _authorizationService;

        private readonly Dictionary<Models.OperationType, Operation> _operations;  // TODO(peddavid): Should this be immutable?

        public WebSocketsController(
            IBoardService boardService,
            StringWebSocketsSessionManager sessionManager,
            IAuthorizationService authorizationService,
            IFigureIdService figureIdService,
            IImageService imageService,
            ILineService lineService
        ) {
            _boardService = boardService;
            _sessionManager = sessionManager;
            _authorizationService = authorizationService;
            _imageOperations = new ImageOperations(imageService, figureIdService, authorizationService);
            _lineOperations = new LineOperations(lineService, figureIdService, authorizationService);

            _operations = new Dictionary<Models.OperationType, Operation>() {
                { Models.OperationType.CREATE_IMAGE, _imageOperations.CreateImage },
                { Models.OperationType.DELETE_IMAGE, _imageOperations.DeleteImage },
                { Models.OperationType.ALTER_IMAGE, _imageOperations.UpdateImage },
                { Models.OperationType.CREATE_LINE, _lineOperations.CreateLine },
                { Models.OperationType.DELETE_LINE, _lineOperations.DeleteLine },
                { Models.OperationType.ALTER_LINE, _lineOperations.UpdateLine }
            };
        }

        [HttpGet("{roomId}")]
        [AllowAnonymous]
        public async Task Index(long roomId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(roomId), Policies.ReadBoardPolicy)) {
                await Challenge().ExecuteResultAsync(ControllerContext);
                return;
            }

            if(!await _boardService.ExistsAsync(roomId)) {
                HttpContext.Response.StatusCode = 404;
                return;
            }

            if(HttpContext.WebSockets.IsWebSocketRequest) {
                StringWebSocket webSocket = await HttpContext.WebSockets.AcceptStringWebSocketAsync(User);

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
