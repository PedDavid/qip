using API.Interfaces;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly PointerOperation _pointerOperations;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<WebSocketsController> _logger;

        private readonly Dictionary<Models.OperationType, Operation> _operations;  // TODO(peddavid): Should this be immutable?

        public WebSocketsController(
            IBoardService boardService,
            StringWebSocketsSessionManager sessionManager,
            IAuthorizationService authorizationService,
            IFigureIdService figureIdService,
            IImageService imageService,
            ILineService lineService,
            ILoggerFactory logger
        ) {
            _boardService = boardService;
            _sessionManager = sessionManager;
            _authorizationService = authorizationService;
            _imageOperations = new ImageOperations(imageService, figureIdService, authorizationService, new Logger<ImageOperations>(logger));
            _lineOperations = new LineOperations(lineService, figureIdService, authorizationService, new Logger<LineOperations>(logger));
            _pointerOperations = new PointerOperation(authorizationService, new Logger<PointerOperation>(logger));
            _logger = new Logger<WebSocketsController>(logger);

            _operations = new Dictionary<Models.OperationType, Operation>() {
                { Models.OperationType.CREATE_IMAGE, _imageOperations.CreateImage },
                { Models.OperationType.DELETE_IMAGE, _imageOperations.DeleteImage },
                { Models.OperationType.ALTER_IMAGE, _imageOperations.UpdateImage },
                { Models.OperationType.CREATE_LINE, _lineOperations.CreateLine },
                { Models.OperationType.DELETE_LINE, _lineOperations.DeleteLine },
                { Models.OperationType.ALTER_LINE, _lineOperations.UpdateLine },
                { Models.OperationType.POINT_TO, _pointerOperations.PointTo }
            };
        }

        /// <summary>
        /// Creates a WebSockets connection 
        /// </summary>
        /// <param name="roomId">Id of the room, equivalent to the id of the associated Board</param>
        /// <response code="101">WebSockets connection succeeds</response>
        /// <response code="400">If the wrong protocol was used</response>
        /// <response code="401">If the user needs authentication</response>
        /// <response code="403">If the user does not have authorization</response>
        /// <response code="404">If the Board associated with the room does not exist</response>
        [HttpGet("{roomId}")]
        [AllowAnonymous]
        [ProducesResponseType(101)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task Connect(long roomId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(roomId), Policies.ReadBoardPolicy)) {
                _logger.LogWarning(LoggingEvents.ConnectWebSocketNotAuthorized, "Connect({roomId}) NOT AUTHORIZED {user_id}", roomId, User.GetNameIdentifier());
                await Challenge().ExecuteResultAsync(ControllerContext);
                return;
            }

            if(!await _boardService.ExistsAsync(roomId)) {
                _logger.LogWarning(LoggingEvents.ConnectWebSocketNotFound, "Connect({roomId}) NOT FOUND", roomId);
                HttpContext.Response.StatusCode = 404;
                return;
            }

            if(HttpContext.WebSockets.IsWebSocketRequest) {
                _logger.LogInformation(LoggingEvents.ConnectWebSocket, "Connect WebSocket {roomId}", roomId);
                StringWebSocket webSocket = await HttpContext.WebSockets.AcceptStringWebSocketAsync(User);

                var session = _sessionManager.Register(roomId, webSocket);
                var swsopers = new StringWebSocketsOperations(roomId, webSocket, session, _operations);

                await swsopers.AcceptRequests();
            }
            else {
                _logger.LogDebug(LoggingEvents.ConnectWebSocketWrongProtocol, "Connect({roomId}) WRONG PROTOCOL", roomId);
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
