using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IODomain.Input;
using IODomain.Extensions;
using IODomain.Output;
using API.Interfaces.IRepositories;
using API.Domain;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers {
    [Route("api/boards/{boardId}/[controller]")]
    public class UsersBoardsController : Controller {
        private readonly IUsersBoardsRepository _userBoardsRepository;

        public UsersBoardsController(IUsersBoardsRepository userBoardsRepository) {
            this._userBoardsRepository = userBoardsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<OutUserBoard_User>> GetAll(long boardId, string search, long index = 0, long size = 10) {
            IEnumerable<UserBoard_User> userBoards = await _userBoardsRepository.GetAllUsersAsync(boardId, index, size, search);

            return userBoards.Select(UserBoard_UserExtensions.Out);
        }

        [HttpGet("{userId}", Name = "GetUserBoard")]
        public async Task<IActionResult> GetById(long boardId, long userId) {
            UserBoard_User user = await _userBoardsRepository.FindUserAsync(boardId, userId);

            if(user == null) {
                return NotFound();
            }

            return new ObjectResult(user.Out());
        }

        [HttpPost]
        public async Task<IActionResult> Create(long boardId, [FromBody] InUserBoard inputUserBoard) {
            if(inputUserBoard == null || inputUserBoard.BoardId != boardId) {
                return BadRequest();
            }

            UserBoard userBoard = new UserBoard().In(inputUserBoard);
            await _userBoardsRepository.AddAsync(userBoard);

            return CreatedAtRoute("GetUserBoard", new { boardId = userBoard.BoardId, userId = userBoard.UserId }, inputUserBoard);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(long boardId, long userId, [FromBody] InUserBoard inputUserBoard) {
            if(inputUserBoard == null || inputUserBoard.BoardId != boardId || inputUserBoard.UserId != userId) {
                return BadRequest();
            }

            UserBoard user = await _userBoardsRepository.FindAsync(boardId, userId);
            if(user == null) {
                return NotFound();
            }

            user.In(inputUserBoard);

            await _userBoardsRepository.UpdateAsync(user);
            return new NoContentResult();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(long boardId, long userId) {
            UserBoard user = await _userBoardsRepository.FindAsync(boardId, userId);
            if(user == null) {
                return NotFound();
            }

            await _userBoardsRepository.RemoveAsync(boardId, userId);
            return new NoContentResult();
        }
    }
}
