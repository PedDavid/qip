using API.Domain;
using API.Filters;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public UsersBoardsController(IUsersBoardsService userBoardsService, IAuthorizationService authorizationService) {
            _userBoardsService = userBoardsService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(long boardId, string search, long index = 0, long size = 10) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy))
                return Challenge();

            IEnumerable<UserBoard_User> userBoards = await _userBoardsService.GetAllUsersAsync(boardId, index, size, search);

            return Ok(userBoards.Select(UserBoard_UserExtensions.Out));
        }

        [HttpGet("{userId}", Name = "GetUserBoard")]
        public async Task<IActionResult> GetById(long boardId, string userId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy))
                return Challenge();

            UserBoard_User userboard = await _userBoardsService.GetUserAsync(boardId, userId);

            if(userboard == null) {
                //$"The UserBoard with board id {boardId} and user id {userId} not exists"
                return NotFound();
            }

            return Ok(userboard.Out());
        }

        [HttpPost]
        public async Task<IActionResult> Create(long boardId, [FromBody] InUpdateUserBoard inUserBoard) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy))
                return Challenge();

            if(inUserBoard == null) {
                return BadRequest();
            }

            if(inUserBoard.BoardId != boardId) {
                //$"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inputUserBoard.BoardId}"
                return BadRequest();
            }

            UserBoard userBoard = new UserBoard().In(inUserBoard);

            await _userBoardsService.CreateAsync(userBoard);

            return CreatedAtRoute("GetUserBoard", new { boardId = boardId, userId = userBoard.UserId }, userBoard);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(long boardId, string userId, [FromBody] InUpdateUserBoard inputUserBoard) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy))
                return Challenge();

            if(inputUserBoard == null) {
                return BadRequest();
            }

            if(inputUserBoard.BoardId != boardId) {
                //$"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inputUserBoard.BoardId}"
                return BadRequest();
            }

            if(inputUserBoard.UserId != userId) {
                //$"The user id present on update is different of the expected. {Environment.NewLine}Expected: {userId}{Environment.NewLine}Current: {inputUserBoard.UserId}"
                return BadRequest();
            }

            UserBoard user = await _userBoardsService.GetAsync(userId, boardId);
            if(user == null) {
                //$"The UserBoard with board id {boardId} and user id {userId} not exists"
                return NotFound();
            }

            user.In(inputUserBoard);

            await _userBoardsService.UpdateAsync(user);

            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(long boardId, string userId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy))
                return Challenge();

            if(!await _userBoardsService.ExistsAsync(userId, boardId)) {
                //$"The UserBoard with board id {boardId} and user id {userId} not exists"
                return NotFound();
            }

            await _userBoardsService.DeleteAsync(boardId, userId);

            return NoContent();
        }
    }
}
