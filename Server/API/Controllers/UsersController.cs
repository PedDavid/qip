using API.Domain;
using API.Filters;
using API.Interfaces;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, IUsersBoardsService usersBoardsService, IAuthorizationService authorizationService, ILogger<UsersController> logger) {
            _userService = userService;
            _usersBoardsService = usersBoardsService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<OutUser>> GetAll(string search, long index = 0, long size = 10) {
            _logger.LogInformation(LoggingEvents.ListUsers, "Listing page {index} of Users with size {size}", index, size);
            IEnumerable<User> users = await _userService.GetAllAsync(index, size, search);
            return users.Select(UserExtensions.Out);
        }

        [HttpGet("{userId}/boards")]
        public async Task<IActionResult> GetAllBoards(string userId, string search, long index = 0, long size = 10) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.ListUserBoardsNotAuthorized, "GetAllBoards({userId}) NOT AUTHORIZED {user_id}", userId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.ListUserBoards, "Listing all Users");
            IEnumerable<UserBoard_Board> userBoards = await _usersBoardsService.GetAllBoardsAsync(userId, index, size, search);

            return Ok(userBoards.Select(UserBoard_BoardExtensions.Out));
        }

        [HttpGet("{userId}/boards/{boardId}")]
        public async Task<IActionResult> GetBoardById(string userId, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.GetUserBoardNotAuthorized, "GetBoardById({userId}, {boardId}) NOT AUTHORIZED {user_id}", userId, boardId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.GetUserBoard, "Getting UserBoard of the user {userId} and board {boardId}", userId, boardId);
            UserBoard_Board userBoard = await _usersBoardsService.GetBoardAsync(userId, boardId);

            if(userBoard == null) {
                //$"The UserBoard with board id {boardId} and user id {userId} not exists"
                _logger.LogWarning(LoggingEvents.GetUserBoardNotFound, "GetByIdAsync({userId}, {boardId}) NOT FOUND", userId, boardId);
                return NotFound();
            }

            return Ok(userBoard.Out());
        }
    }
}
