using API.Domain;
using API.Filters;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IEnumerable<OutUser>> GetAll(string search, long index = 0, long size = 10) {
            IEnumerable<User> users = await _userService.GetAllAsync(index, size, search);
            return users.Select(UserExtensions.Out);
        }

        [HttpGet("{userId}/boards")]
        public async Task<IActionResult> GetAll(string userId, string search, long index = 0, long size = 10) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy))
                return Challenge();

            IEnumerable<UserBoard_Board> userBoards = await _usersBoardsService.GetAllBoardsAsync(userId, index, size, search);

            return Ok(userBoards.Select(UserBoard_BoardExtensions.Out));
        }

        [HttpGet("{userId}/boards/{boardId}")]
        public async Task<IActionResult> GetById(string userId, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy))
                return Challenge();

            UserBoard_Board userBoard = await _usersBoardsService.GetBoardAsync(userId, boardId);

            if(userBoard == null) {
                //$"The UserBoard with board id {boardId} and user id {userId} not exists"
                return NotFound();
            }

            return Ok(userBoard.Out());
        }
    }
}
