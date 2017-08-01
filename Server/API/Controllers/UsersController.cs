using API.Filters;
using API.Interfaces.IServices;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers {
    [ServicesExceptionFilter]
    [Route("api/[controller]")]
    public class UsersController : Controller {
        private readonly IUserService _userService;
        private readonly IUsersBoardsService _usersBoardsService;

        public UsersController(IUserService userService, IUsersBoardsService usersBoardsService) {
            _userService = userService;
            _usersBoardsService = usersBoardsService;
        }

        [HttpGet]
        public Task<IEnumerable<OutUser>> GetAll(string search, long index = 0, long size = 10) {
            return _userService.GetAllAsync(index, size, search);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public Task<OutUser> GetById(long id) {
            return _userService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InUser inputUser) {
            OutUser user = await _userService.CreateAsync(inputUser);
            return CreatedAtRoute("GetUser", new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] InUser inputUser) {
            await _userService.UpdateAsync(id, inputUser);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id) {
            await _userService.DeleteAsync(id);
            return new NoContentResult();
        }

        [HttpGet("{userId}/boards")]
        public Task<IEnumerable<OutUserBoard_Board>> GetAll(long userId, string search, long index = 0, long size = 10) {
            return _usersBoardsService.GetAllBoardsAsync(userId, index, size, search);
        }

        [HttpGet("{userId}/boards/{boardId}")]
        public Task<OutUserBoard_Board> GetById(long userId, long boardId) {
            return _usersBoardsService.GetBoardAsync(userId, boardId);
        }
    }
}
