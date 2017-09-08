using API.Domain;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using API.Interfaces.ServicesExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services {
    class UsersBoardsService : IUsersBoardsService {
        private readonly IUsersBoardsRepository _usersBoardsRepository;
        private readonly IUserService _userService;
        private readonly IBoardService _boardService;

        public UsersBoardsService(IUsersBoardsRepository usersBoardsRepository, IUserService userService, IBoardService boardService) {
            _usersBoardsRepository = usersBoardsRepository;
            _userService = userService;
            _boardService = boardService;
        }

        public async Task CreateAsync(UserBoard userBoard) {
            if(userBoard == null) {
                throw new ArgumentNullException("Argument userBoard can not be null");
            }

            if(userBoard.Permission == BoardPermission.Owner) {
                throw new InvalidFieldsException("It is not possible give owner permissions.");
            }

            if(!await _boardService.ExistsAsync(userBoard.BoardId)) {
                throw new NotFoundException($"The Board with id {userBoard.BoardId} not exists");
            }

            if(!await _userService.ExistsAsync(userBoard.UserId)) {
                throw new NotFoundException($"The User with id {userBoard.UserId} not exists");
            }

            await _usersBoardsRepository.AddAsync(userBoard);
        }

        public async Task DeleteAsync(long boardId, string userId) {
            UserBoard userBoard = await _usersBoardsRepository.FindAsync(boardId, userId);

            if(userBoard == null) {
                throw new NotFoundException($"The UserBoard with board id {boardId} and user id {userId} not exists");
            }

            if(userBoard.Permission == BoardPermission.Owner) {
                throw new InvalidChangeException($"It is not possible for the owner to withdraw his own permissions");
            }

            await _usersBoardsRepository.RemoveAsync(boardId, userId);
        }

        public Task<IEnumerable<UserBoard_Board>> GetAllBoardsAsync(string userId, long index, long size, string search) {
            return _usersBoardsRepository.GetAllBoardsAsync(userId, index, size, search);
        }

        public async Task<IEnumerable<UserBoard_User>> GetAllUsersAsync(long boardId, long index, long size, string search) {
            IEnumerable<UserBoard_User> userBoards = await _usersBoardsRepository.GetAllUsersAsync(boardId, index, size, search);

            string userIds = String.Join(" OR ", userBoards.Select(ub => ub.User.User_id).ToArray());

            if(string.IsNullOrWhiteSpace(userIds))
                return userBoards;

            var sch = $"user_id:({userIds})";

            if(search != null) {
                sch = $"{sch} AND {search}";
            }

            IEnumerable<User> users = await _userService.GetAllAsync(index, size, sch);

            return userBoards.Join(
                users,
                ub => ub.User.User_id,
                u => u.User_id,
                (ub, u) => {
                    ub.User = u;
                    return ub;
                }
            );
        }

        public Task<UserBoard_Board> GetBoardAsync(string userId, long boardId) {
            return _usersBoardsRepository.FindBoardAsync(userId, boardId);
        }

        public async Task<UserBoard_User> GetUserAsync(long boardId, string userId) {
            UserBoard_User user = await _usersBoardsRepository.FindUserAsync(boardId, userId);

            user.User = await _userService.GetAsync(userId);

            return user;
        }

        public async Task UpdateAsync(UserBoard userBoard) {
            if(userBoard == null) {
                throw new ArgumentNullException("Argument userBoard can not be null");
            }

            UserBoard user = await _usersBoardsRepository.FindAsync(userBoard.BoardId, userBoard.UserId);
            if(user == null) { // TODO Rever a repetição do pedido para obter o User
                throw new NotFoundException($"The UserBoard with board id {userBoard.BoardId} and user id {userBoard.UserId} not exists");
            }

            if(user.Permission != BoardPermission.Owner && userBoard.Permission == BoardPermission.Owner) {
                throw new InvalidChangeException("It is not possible change the permissions for owner.");
            }

            if(user.Permission == BoardPermission.Owner && userBoard.Permission != BoardPermission.Owner) {
                throw new InvalidChangeException("It is not possible change the permissions owner.");
            }

            await _usersBoardsRepository.UpdateAsync(user);
        }

        public Task<BoardPermission> GetPermissionAsync(string userId, long boardId) {
            return _usersBoardsRepository.FindPermissionAsync(userId, boardId);
        }

        public Task<UserBoard> GetAsync(string userId, long boardId) {
            return _usersBoardsRepository.FindAsync(boardId, userId);
        }

        public Task<bool> ExistsAsync(string userId, long boardId) {
            return _usersBoardsRepository.ExistsAsync(boardId, userId);
        }
    }
}
