using QIP.Domain;
using QIP.API.Filters;
using QIP.Public;
using QIP.Public.IServices;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using QIP.IODomain.Extensions;
using QIP.IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QIP.API.Controllers {
    [ServicesExceptionFilter]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : Controller {
        private readonly IUserService _userService;
        private readonly IUsersBoardsService _usersBoardsService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, IUsersBoardsService usersBoardsService, IAuthorizationService authorizationService, ILogger<UsersController> logger) {
            _userService = userService;
            _usersBoardsService = usersBoardsService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a List of Users
        /// </summary>
        /// <param name="search">Query for search by Users</param>
        /// <param name="index">Number of the page</param>
        /// <param name="size">Number of Users for page</param>
        /// <returns>Required List of Users</returns>
        /// <response code="200">Returns the required list of Users</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OutUser>), 200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<OutUser>> GetAll(string search, long index = 0, long size = 10) {
            _logger.LogInformation(LoggingEvents.ListUsers, "Listing page {index} of Users with size {size}", index, size);
            IEnumerable<User> users = await _userService.GetAllAsync(index, size, search);
            return users.Select(UserExtensions.Out);
        }

        /// <summary>
        /// Returns a List of user's Boards
        /// </summary>
        /// <param name="userId">User id to which the Boards belong</param>
        /// <param name="search">Query for search by Boards</param>
        /// <param name="index">Number of the page</param>
        /// <param name="size">Number of Boards for page</param>
        /// <returns>Required List of Users</returns>
        /// <response code="200">Returns the required list of Boards</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have authorization</response>
        [HttpGet("{userId}/boards")]
        [ProducesResponseType(typeof(IEnumerable<OutUserBoard_Board>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAllBoards(string userId, string search, long index = 0, long size = 10) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.ListUserBoardsNotAuthorized, "GetAllBoards({userId}) NOT AUTHORIZED {user_id}", userId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.ListUserBoards, "Listing all Users");
            IEnumerable<UserBoard_Board> userBoards = await _usersBoardsService.GetAllBoardsAsync(userId, index, size, search);

            return Ok(userBoards.Select(UserBoard_BoardExtensions.Out));
        }

        /// <summary>
        /// Returns a specific user's Board.
        /// </summary>
        /// <param name="userId">Id of the user to which the Board belongs</param>
        /// <param name="boardId">Id of the Board to return</param>
        /// <returns>Required Board</returns>
        /// <response code="200">Returns the required Board</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have authorization</response>
        /// <response code="404">If the user's Board not exists</response>
        [HttpGet("{userId}/boards/{boardId}")]
        [ProducesResponseType(typeof(OutUserBoard_Board), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBoardById(string userId, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.GetUserBoardNotAuthorized, "GetBoardById({userId}, {boardId}) NOT AUTHORIZED {user_id}", userId, boardId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.GetUserBoard, "Getting UserBoard of the user {userId} and board {boardId}", userId, boardId);
            UserBoard_Board userBoard = await _usersBoardsService.GetBoardAsync(userId, boardId);

            if(userBoard == null) {
                //$"The UserBoard with board id {boardId} and user id {userId} not exists"
                _logger.LogWarning(LoggingEvents.GetUserBoardNotFound, "GetByIdAsync({userId}, {boardId}) NOT FOUND", userId, boardId);
                return NotFound();
            }

            return Ok(userBoard.Out());
        }
    }
}
