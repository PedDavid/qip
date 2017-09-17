using QIP.Domain;
using QIP.API.Filters;
using QIP.Public;
using QIP.Public.IServices;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using QIP.IODomain.Extensions;
using QIP.IODomain.Input;
using QIP.IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QIP.API.Controllers {
    [ValidateModel]
    [ServicesExceptionFilter]
    [Produces("application/json")]
    [Route("api/boards/{boardId}/[controller]")]
    public class UsersBoardsController : Controller {
        private readonly IUsersBoardsService _userBoardsService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<UsersBoardsController> _logger;

        public UsersBoardsController(IUsersBoardsService userBoardsService, IAuthorizationService authorizationService, ILogger<UsersBoardsController> logger) {
            _userBoardsService = userBoardsService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a List of board's Users
        /// </summary>
        /// <param name="boardId">Board id to which the Users belong</param>
        /// <param name="search">Query for search by Users</param>
        /// <param name="index">Number of the page</param>
        /// <param name="size">Number of Users for page</param>
        /// <returns>Required List of Users</returns>
        /// <response code="200">Returns the required list of Users</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have authorization</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OutUserBoard_User>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAll(long boardId, string search, long index = 0, long size = 10) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.ListBoardUsersNotAuthorized, "GetAll({boardId}) NOT AUTHORIZED {user_id}", boardId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.ListBoardUsers, "Listing page {index} of BoardUsers with size {size}", index, size);
            IEnumerable<UserBoard_User> userBoards = await _userBoardsService.GetAllUsersAsync(boardId, index, size, search);

            return Ok(userBoards.Select(UserBoard_UserExtensions.Out));
        }

        /// <summary>
        /// Returns a specific board's User.
        /// </summary>
        /// <param name="boardId">Board id to which the Users belong</param>
        /// <param name="userId">Id of the User to return</param>
        /// <returns>Required User</returns>
        /// <response code="200">Returns the required User</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have authorization</response>
        /// <response code="404">If the board's User not exists</response>
        [HttpGet("{userId}", Name = "GetUserBoard")]
        [ProducesResponseType(typeof(OutUserBoard_User), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(long boardId, string userId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.GetBoardUserNotAuthorized, "GetById({boardId}, {userId}) NOT AUTHORIZED {user_id}", boardId, userId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.GetBoardUser, "Getting BoardUser of board {boardId} and user {userId}", boardId, userId);
            UserBoard_User userboard = await _userBoardsService.GetUserAsync(boardId, userId);

            if(userboard == null) {
                //$"The UserBoard with board id {boardId} and user id {userId} not exists"
                _logger.LogWarning(LoggingEvents.GetBoardUserNotFound, "GetById({boardId}, {userId}) NOT FOUND", boardId, userId);
                return NotFound();
            }

            return Ok(userboard.Out());
        }

        /// <summary>
        /// Associate a specific User with a specific Board
        /// </summary>
        /// <remarks>
        /// UserBoard is an association between a specific Board and a specific User
        /// 
        /// Sample request:
        ///
        ///     POST /api/Board/0/UsersBoards
        ///     Content-Type: application/json
        ///     Authorization: Bearer {ACCESS_TOKEN}
        /// 
        ///     {
        ///         "boardId": 0,
        ///         "userId": "auth0|1234567890",
        ///         "permission": 1
        ///     }
        ///
        /// </remarks>
        /// <param name="boardId">Id of the Board to associate</param>
        /// <param name="inUserBoard">Information of the UserBoard to create</param>
        /// <returns>A newly-created UserBoard</returns>
        /// <response code="201">Returns the newly-created UserBoard</response>
        /// <response code="400">If there is inconsistent information or the inUserBoard is null</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have authorization</response>
        [HttpPost]
        [ProducesResponseType(typeof(OutUserBoard), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Create(long boardId, [FromBody] InCreateUserBoard inUserBoard) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.InsertUserBoardNotAuthorized, "Create({boardId}) NOT AUTHORIZED {user_id}", boardId, User.GetNameIdentifier());
                return Challenge();
            }

            if(inUserBoard == null) {
                _logger.LogDebug(LoggingEvents.InsertUserBoardWithoutBody, "Create({boardId}) WITHOUT BODY", boardId);
                return BadRequest();
            }

            if(inUserBoard.BoardId != boardId) {
                //$"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inputUserBoard.BoardId}"
                _logger.LogDebug(LoggingEvents.InsertUserBoardWrongBoardId, "Create({boardId}) WRONG BOARD ID {wrongBoardId}", boardId, inUserBoard.BoardId);
                return BadRequest();
            }

            UserBoard userBoard = new UserBoard().In(inUserBoard);

            await _userBoardsService.CreateAsync(userBoard);
            _logger.LogInformation(LoggingEvents.InsertUserBoard, "UserBoard of user {userId} and board {boardId} Created", userBoard.UserId, boardId);

            return CreatedAtRoute("GetUserBoard", new { boardId = boardId, userId = userBoard.UserId }, userBoard.Out());
        }

        /// <summary>
        /// Updates the association between a specific Board and a specific User
        /// </summary>
        /// <remarks>
        /// UserBoard is an association between a specific Board and a specific User
        /// 
        /// Sample request:
        ///
        ///     PUT /api/Board/0/UsersBoards/auth0%7C1234567890
        ///     Content-Type: application/json
        ///     Authorization: Bearer {ACCESS_TOKEN}
        /// 
        ///     {
        ///         "boardId": 0,
        ///         "userId": "auth0|1234567890",
        ///         "permission": 2
        ///     }
        ///
        /// </remarks>
        /// <param name="boardId">Id of the associated Board</param>
        /// <param name="userId">Id of the associated User</param>
        /// <param name="inputUserBoard">Information of the UserBoard to update</param>
        /// <response code="204">UserBoard update succeeds</response>
        /// <response code="400">If there is inconsistent information or the inputUserBoard is null</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have authorization</response>
        /// <response code="404">If the UserBoard not exists</response>
        [HttpPut("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(long boardId, string userId, [FromBody] InUpdateUserBoard inputUserBoard) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.UpdateUserBoardNotAuthorized, "Update({boardId}, {userId}) NOT AUTHORIZED {user_id}", boardId, userId, User.GetNameIdentifier());
                return Challenge();
            }

            if(inputUserBoard == null) {
                _logger.LogDebug(LoggingEvents.UpdateUserBoardWithoutBody, "Update({boardId}, {userId}) WITHOUT BODY", boardId, userId);
                return BadRequest();
            }

            if(inputUserBoard.BoardId != boardId) {
                //$"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inputUserBoard.BoardId}"
                _logger.LogDebug(LoggingEvents.UpdateUserBoardWrongBoardId, "Update({boardId}, {userId}) WRONG BOARD ID {wrongBoardId}", boardId, userId, inputUserBoard.BoardId);
                return BadRequest();
            }

            if(inputUserBoard.UserId != userId) {
                //$"The user id present on update is different of the expected. {Environment.NewLine}Expected: {userId}{Environment.NewLine}Current: {inputUserBoard.UserId}"
                _logger.LogDebug(LoggingEvents.UpdateUserBoardWrongUserId, "Update({boardId}, {userId}) WRONG USER ID {wrongUserId}", boardId, userId, inputUserBoard.UserId);
                return BadRequest();
            }

            UserBoard user = await _userBoardsService.GetAsync(userId, boardId);
            if(user == null) {
                //$"The UserBoard with board id {boardId} and user id {userId} not exists"
                _logger.LogWarning(LoggingEvents.UpdateUserBoardNotFound, "Update({boardId}, {userId}) NOT FOUND", boardId, userId);
                return NotFound();
            }

            user.In(inputUserBoard);

            await _userBoardsService.UpdateAsync(user);
            _logger.LogInformation(LoggingEvents.UpdateUserBoard, "UserBoard of user {userId} and board {boardId} Updated", userId, boardId);

            return NoContent();
        }

        /// <summary>
        /// Disassociate a specific user from a specific frame
        /// </summary>
        /// <param name="boardId">Id of the Board to be disassociated</param>
        /// <param name="userId">Id of the User to be disassociated</param>
        /// <response code="204">The User and the Board were successfully disassociated</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have authorization</response>
        /// <response code="404">If the association between the user and the Board does not exist</response>
        [HttpDelete("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(long boardId, string userId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.DeleteUserBoardNotAuthorized, "Delete({boardId}, {userId}) NOT AUTHORIZED {user_id}", boardId, userId, User.GetNameIdentifier());
                return Challenge();
            }

            if(!await _userBoardsService.ExistsAsync(userId, boardId)) {
                //$"The UserBoard with board id {boardId} and user id {userId} not exists"
                _logger.LogWarning(LoggingEvents.DeleteUserBoardNotFound, "Delete({boardId}, {userId}) NOT FOUND", boardId, userId);
                return NotFound();
            }

            await _userBoardsService.DeleteAsync(boardId, userId);
            _logger.LogInformation(LoggingEvents.DeleteUserBoard, "UserBoard of user {userId} and board {boardId} Deleted", userId, boardId);

            return NoContent();
        }
    }
}
