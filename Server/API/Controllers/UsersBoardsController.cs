using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Models.IRepositories;
using API.Models;
using API.Models.Input;
using API.Models.Extensions;
using API.Models.Output;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers {
    [Route("api/boards/{boardId}/[controller]")]
    public class UsersBoardsController : Controller {
        private readonly IUsersBoardsRepository _userBoardsRepository;

        public UsersBoardsController(IUsersBoardsRepository userBoardsRepository) {
            this._userBoardsRepository = userBoardsRepository;
        }

        [HttpGet]
        public IEnumerable<OutUserBoard_User> GetAll(long boardId) {
            return _userBoardsRepository
                .GetAllUsers(boardId)
                .Select(UserBoard_UserExtensions.Out);
        }

        [HttpGet("{userId}", Name = "GetUserBoard")]
        public IActionResult GetById(long boardId, long userId) {
            UserBoard_User user = _userBoardsRepository.FindUser(boardId, userId);

            if(user == null) {
                return NotFound();
            }

            return new ObjectResult(user.Out());
        }

        [HttpPost]
        public IActionResult Create(long boardId, [FromBody] InUserBoard inputUserBoard) {
            if(inputUserBoard == null || inputUserBoard.BoardId != boardId) {
                return BadRequest();
            }

            UserBoard userBoard = new UserBoard().In(inputUserBoard);
            _userBoardsRepository.Add(userBoard);

            return CreatedAtRoute("GetUserBoard", new { boardId = userBoard.BoardId, userId = userBoard.UserId }, inputUserBoard);
        }

        [HttpPut("{userId}")]
        public IActionResult Update(long boardId, long userId, [FromBody] InUserBoard inputUserBoard) {
            if(inputUserBoard == null || inputUserBoard.BoardId != boardId || inputUserBoard.UserId != userId) {
                return BadRequest();
            }

            UserBoard user = _userBoardsRepository.Find(boardId, userId);
            if(user == null) {
                return NotFound();
            }

            user.In(inputUserBoard);

            _userBoardsRepository.Update(user);
            return new NoContentResult();
        }

        [HttpDelete("{userId}")]
        public IActionResult Delete(long boardId, long userId) {
            UserBoard user = _userBoardsRepository.Find(boardId, userId);
            if(user == null) {
                return NotFound();
            }

            _userBoardsRepository.Remove(boardId, userId);
            return new NoContentResult();
        }
    }
}
