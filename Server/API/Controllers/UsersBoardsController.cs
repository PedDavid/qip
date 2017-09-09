using API.Domain;
using API.Filters;
using API.Interfaces;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers {
    [ValidateModel]
    [ServicesExceptionFilter]
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

        [HttpGet]
        public async Task<IActionResult> GetAll(long boardId, string search, long index = 0, long size = 10) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.ListBoardUsersNotAuthorized, "GetAll({boardId}) NOT AUTHORIZED {user_id}", boardId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.ListBoardUsers, "Listing page {index} of BoardUsers with size {size}", index, size);
            IEnumerable<UserBoard_User> userBoards = await _userBoardsService.GetAllUsersAsync(boardId, index, size, search);

            return Ok(userBoards.Select(UserBoard_UserExtensions.Out));
        }

        [HttpGet("{userId}", Name = "GetUserBoard")]
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

        [HttpPost]
        public async Task<IActionResult> Create(long boardId, [FromBody] InUpdateUserBoard inUserBoard) {
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

            return CreatedAtRoute("GetUserBoard", new { boardId = boardId, userId = userBoard.UserId }, userBoard);
        }

        [HttpPut("{userId}")]
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

        [HttpDelete("{userId}")]
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
