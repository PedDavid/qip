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
    [Route("api/[controller]")]
    public class UsersController : Controller {
        private readonly IUserService _userService;
        private readonly IUsersBoardsService _usersBoardsService;
        private readonly IAuthorizationService _authorizationService;

        public UsersController(IUserService userService, IUsersBoardsService usersBoardsService, IAuthorizationService authorizationService) {
            _userService = userService;
            _usersBoardsService = usersBoardsService;
            _authorizationService = authorizationService;
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
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(""), Policies.UserIsOwnPolicy)) // REVER UserId parametro
                return new ChallengeResult();

            await _userService.UpdateAsync(id, inputUser);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(""), Policies.UserIsOwnPolicy)) // REVER UserId parametro
                return new ChallengeResult();

            await _userService.DeleteAsync(id);
            return new NoContentResult();
        }

        [HttpGet("{userId}/boards")]
        public async Task<IActionResult> GetAll(long userId, string search, long index = 0, long size = 10) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(""), Policies.UserIsOwnPolicy)) // REVER UserId parametro
                return new ChallengeResult();

            IEnumerable<OutUserBoard_Board> userBoards = await _usersBoardsService.GetAllBoardsAsync(userId, index, size, search);

            return Ok(userBoards);
        }

        [HttpGet("{userId}/boards/{boardId}")]
        public async Task<IActionResult> GetById(long userId, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(""), Policies.UserIsOwnPolicy)) // REVER UserId parametro
                return new ChallengeResult();

            OutUserBoard_Board userBoard = await _usersBoardsService.GetBoardAsync(userId, boardId);

            return Ok(userBoard);
        }
    }
}
