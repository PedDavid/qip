using API.Filters;
using API.Interfaces.IServices;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers {
    [ServicesExceptionFilter]
    [Route("api/boards/{boardId}/[controller]")]
    public class UsersBoardsController : Controller {
        private readonly IUsersBoardsService _userBoardsService;

        public UsersBoardsController(IUsersBoardsService userBoardsService) {
            _userBoardsService = userBoardsService;
        }

        [HttpGet]
        public Task<IEnumerable<OutUserBoard_User>> GetAll(long boardId, string search, long index = 0, long size = 10) {
            return _userBoardsService.GetAllUsersAsync(boardId, index, size, search);
        }

        [HttpGet("{userId}", Name = "GetUserBoard")]
        public Task<OutUserBoard_User> GetById(long boardId, long userId) {
            return _userBoardsService.GetUserAsync(boardId, userId);
        }

        [HttpPost]
        public async Task<IActionResult> Create(long boardId, [FromBody] InUserBoard inputUserBoard) {
            OutUserBoard userBoard = await _userBoardsService.CreateAsync(boardId, inputUserBoard);
            return CreatedAtRoute("GetUserBoard", new { boardId = boardId, userId = userBoard.UserId }, userBoard);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(long boardId, long userId, [FromBody] InUserBoard inputUserBoard) {
            await _userBoardsService.UpdateAsync(boardId, userId, inputUserBoard);
            return new NoContentResult();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(long boardId, long userId) {
            await _userBoardsService.DeleteAsync(boardId, userId);
            return new NoContentResult();
        }
    }
}
