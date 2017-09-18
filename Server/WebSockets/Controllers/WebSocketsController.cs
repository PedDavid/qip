using QIP.Public;
using QIP.Public.IRepositories;
using QIP.Public.IServices;
using QIP.Authorization;
using QIP.Authorization.Extensions;
using QIP.Authorization.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using QIP.WebSockets.Extensions;
using QIP.WebSockets.Operations;
using QIP.WebSockets.StringWebSockets;

namespace QIP.WebSockets.Controllers {
    [Route("ws")]
    public class WebSocketsController : Controller {
        private readonly IBoardService _boardService;
        private readonly StringWebSocketsSessionManager _sessionManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<WebSocketsController> _logger;
        private readonly ILogger<StringWebSocketsOperations> _operationsLogger;

        private readonly Dictionary<Models.OperationType, Operation> _operations;  // TODO(peddavid): Should this be immutable?

        public WebSocketsController(
            IBoardService boardService,
            StringWebSocketsSessionManager sessionManager,
            IAuthorizationService authorizationService,
            IFigureIdService figureIdService,
            IImageService imageService,
            ILineService lineService,
            IFiguresService figuresService,
            ILoggerFactory logger
        ) {
            _boardService = boardService;
            _sessionManager = sessionManager;
            _authorizationService = authorizationService;
            var imageOperations = new ImageOperations(imageService, figureIdService, authorizationService, new Logger<ImageOperations>(logger));
            var lineOperations = new LineOperations(lineService, figureIdService, authorizationService, new Logger<LineOperations>(logger));
            var pointerOperations = new PointerOperation(authorizationService, new Logger<PointerOperation>(logger));
            var figuresOperations = new FiguresOperations(figuresService, authorizationService, new Logger<FiguresOperations>(logger));
            _logger = new Logger<WebSocketsController>(logger);
            _operationsLogger = new Logger<StringWebSocketsOperations>(logger);

            _operations = new Dictionary<Models.OperationType, Operation>() {
                { Models.OperationType.CREATE_IMAGE, imageOperations.CreateImage },
                { Models.OperationType.DELETE_IMAGE, imageOperations.DeleteImage },
                { Models.OperationType.ALTER_IMAGE, imageOperations.UpdateImage },
                { Models.OperationType.CREATE_LINE, lineOperations.CreateLine },
                { Models.OperationType.DELETE_LINE, lineOperations.DeleteLine },
                { Models.OperationType.ALTER_LINE, lineOperations.UpdateLine },
                { Models.OperationType.POINT_TO, pointerOperations.PointTo },
                { Models.OperationType.BOARD_CLEAN, figuresOperations.DeleteFigures }
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
                var swsopers = new StringWebSocketsOperations(roomId, webSocket, session, _operations, _operationsLogger);

                await swsopers.AcceptRequests();
            }
            else {
                _logger.LogDebug(LoggingEvents.ConnectWebSocketWrongProtocol, "Connect({roomId}) WRONG PROTOCOL", roomId);
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
