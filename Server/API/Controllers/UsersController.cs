using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Interfaces.IRepositories;
using API.Domain;
using IODomain.Output;
using IODomain.Extensions;
using IODomain.Input;

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
        public async Task<IEnumerable<OutUser>> GetAll(long index = 0, long size = 10) {
            IEnumerable<User> users = await _userRepository.GetAllAsync(index, size);

            return users.Select(UserExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetById(long id) {
            User user = await _userRepository.FindAsync(id);
            if(user == null) {
                return NotFound();
            }
            return new ObjectResult(user.Out());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InUser inputUser) {
            if(inputUser == null) {
                return BadRequest();
            }

            User user = new User().In(inputUser);
            long id = await _userRepository.AddAsync(user);

            inputUser.Id = id;
            return CreatedAtRoute("GetUser", new { id = id }, inputUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] InUser inputUser) {
            if(inputUser == null || inputUser.Id != id) {
                return BadRequest();
            }

            User user = await _userRepository.FindAsync(id);
            if(user == null) {
                return NotFound();
            }

            user.In(inputUser);

            await _userRepository.UpdateAsync(user);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id) {
            User user = await _userRepository.FindAsync(id);
            if(user == null) {
                return NotFound();
            }

            await _userRepository.RemoveAsync(id);
            return new NoContentResult();
        }

        [HttpGet("{userId}/boards")]
        public async Task<IEnumerable<OutUserBoard_Board>> GetAll(long userId, long index = 0, long size = 10) {
            IEnumerable<UserBoard_Board> userBoards = await _userBoardsRepository.GetAllBoardsAsync(userId, index, size);

            return userBoards.Select(UserBoard_BoardExtensions.Out);
        }

        [HttpGet("{userId}/boards/{boardId}")]
        public async Task<IActionResult> GetById(long userId, long boardId) {
            UserBoard_Board board = await _userBoardsRepository.FindBoardAsync(userId, boardId);

            if(board == null) {
                return NotFound();
            }

            return new ObjectResult(board.Out());
        }
    }
}
