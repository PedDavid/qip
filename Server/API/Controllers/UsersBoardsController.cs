using Authorization;
using Authorization.Resources;
using API.Filters;
using API.Interfaces.IServices;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers {
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
                return new ChallengeResult();

            IEnumerable < OutUserBoard_User > userBoards = await _userBoardsService.GetAllUsersAsync(boardId, index, size, search);

            return Ok(userBoards);
        }

        [HttpGet("{userId}", Name = "GetUserBoard")]
        public async Task<IActionResult> GetById(long boardId, string userId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy))
                return new ChallengeResult();

            OutUserBoard_User userboard = await _userBoardsService.GetUserAsync(boardId, userId);

            return Ok(userboard);
        }

        [HttpPost]
        public async Task<IActionResult> Create(long boardId, [FromBody] InUserBoard inputUserBoard) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy))
                return new ChallengeResult();

            OutUserBoard userBoard = await _userBoardsService.CreateAsync(boardId, inputUserBoard);

            return CreatedAtRoute("GetUserBoard", new { boardId = boardId, userId = userBoard.UserId }, userBoard);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(long boardId, string userId, [FromBody] InUserBoard inputUserBoard) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy))
                return new ChallengeResult();

            await _userBoardsService.UpdateAsync(boardId, userId, inputUserBoard);

            return new NoContentResult();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(long boardId, string userId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy))
                return new ChallengeResult();

            await _userBoardsService.DeleteAsync(boardId, userId);

            return new NoContentResult();
        }
    }
}
