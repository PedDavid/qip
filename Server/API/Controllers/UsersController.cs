using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Models.Input;
using API.Models.Output;
using API.Models.Extensions;
using API.Interfaces.IRepositories;
using API.Domain;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers {
    [Route("api/[controller]")]
    public class UsersController : Controller {
        private readonly IUserRepository _userRepository;
        private readonly IUsersBoardsRepository _userBoardsRepository;

        public UsersController(IUserRepository userRepository, IUsersBoardsRepository userBoardsRepository) {
            this._userRepository = userRepository;
            this._userBoardsRepository = userBoardsRepository;
        }

        [HttpGet]
        public IEnumerable<OutUser> GetAll() {
            return _userRepository
                .GetAll()
                .Select(UserExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult GetById(long id) {
            User user = _userRepository.Find(id);
            if(user == null) {
                return NotFound();
            }
            return new ObjectResult(user.Out());
        }

        [HttpPost]
        public IActionResult Create([FromBody] InUser inputUser) {
            if(inputUser == null) {
                return BadRequest();
            }

            User user = new User().In(inputUser);
            long id = _userRepository.Add(user);

            inputUser.Id = id;
            return CreatedAtRoute("GetUser", new { id = id }, inputUser);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] InUser inputUser) {
            if(inputUser == null || inputUser.Id != id) {
                return BadRequest();
            }

            User user = _userRepository.Find(id);
            if(user == null) {
                return NotFound();
            }

            user.In(inputUser);

            _userRepository.Update(user);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id) {
            User user = _userRepository.Find(id);
            if(user == null) {
                return NotFound();
            }

            _userRepository.Remove(id);
            return new NoContentResult();
        }

        [HttpGet("{userId}/boards")]
        public IEnumerable<OutUserBoard_Board> GetAll(long userId) {
            return _userBoardsRepository
                .GetAllBoards(userId)
                .Select(UserBoard_BoardExtensions.Out);
        }

        [HttpGet("{userId}/boards/{boardId}")]
        public IActionResult GetById(long userId, long boardId) {
            UserBoard_Board board = _userBoardsRepository.FindBoard(userId, boardId);

            if(board == null) {
                return NotFound();
            }

            return new ObjectResult(board.Out());
        }
    }
}
