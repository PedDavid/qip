using API.Domain;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using API.Interfaces.ServicesExceptions;
using API.Services.Utils;
using IODomain.Extensions;
using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services {
    class UsersBoardsService : IUsersBoardsService {
        private readonly IUsersBoardsRepository _usersBoardsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBoardRepository _boardRepository;

        public UsersBoardsService(IUsersBoardsRepository usersBoardsRepository, IUserRepository userRepository, IBoardRepository boardRepository) {
            _usersBoardsRepository = usersBoardsRepository;
            _userRepository = userRepository;
            _boardRepository = boardRepository;
        }

        public async Task<OutUserBoard> CreateAsync(long boardId, InUserBoard inputUserBoard) {
            if(inputUserBoard == null) {
                throw new MissingInputException();
            }

            Validator<InUserBoard>.Valid(inputUserBoard, GetValidationConfigurations());

            long userId = inputUserBoard.UserId.Value;

            if(inputUserBoard.BoardId != boardId) {
                throw new InconsistentRequestException(
                    $"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inputUserBoard.BoardId}"
                );
            }

            if(!await _userRepository.ExistsAsync(userId)) {
                throw new NotFoundException($"The User with id {userId} not exists");
            }

            if(!await _boardRepository.ExistsAsync(boardId)) {
                throw new NotFoundException($"The Board with id {boardId} not exists");
            }

            UserBoard userBoard = new UserBoard().In(inputUserBoard);
            await _usersBoardsRepository.AddAsync(userBoard);

            return userBoard.Out();
        }

        public async Task DeleteAsync(long boardId, long userId) {
            UserBoard_User user = await _usersBoardsRepository.FindUserAsync(boardId, userId);

            if(user != null) {
                throw new NotFoundException($"The UserBoard with board id {boardId} and user id {userId} not exists");
            }

            if(user.Permission == BoardPermission.Owner) {
                throw new InvalidChangeException($"It is not possible for the owner to withdraw his own permissions");
            }

            await _usersBoardsRepository.RemoveAsync(boardId, userId);
        }

        public async Task<IEnumerable<OutUserBoard_Board>> GetAllBoardsAsync(long userId, long index, long size, string search) {
            IEnumerable<UserBoard_Board> userBoards = await _usersBoardsRepository.GetAllBoardsAsync(userId, index, size, search);

            return userBoards.Select(UserBoard_BoardExtensions.Out);
        }

        public async Task<IEnumerable<OutUserBoard_User>> GetAllUsersAsync(long boardId, long index, long size, string search) {
            IEnumerable<UserBoard_User> userBoards = await _usersBoardsRepository.GetAllUsersAsync(boardId, index, size, search);

            return userBoards.Select(UserBoard_UserExtensions.Out);
        }

        public async Task<OutUserBoard_Board> GetBoardAsync(long userId, long boardId) {
            UserBoard_Board board = await _usersBoardsRepository.FindBoardAsync(userId, boardId);

            if(board == null) {
                throw new NotFoundException($"The UserBoard with board id {boardId} and user id {userId} not exists");
            }

            return board.Out();
        }

        public async Task<OutUserBoard_User> GetUserAsync(long boardId, long userId) {
            UserBoard_User user = await _usersBoardsRepository.FindUserAsync(boardId, userId);

            if(user == null) {
                throw new NotFoundException($"The UserBoard with board id {boardId} and user id {userId} not exists");
            }

            return user.Out();
        }

        public async Task<OutUserBoard> UpdateAsync(long boardId, long userId, InUserBoard inputUserBoard) {
            if(inputUserBoard == null) {
                throw new MissingInputException();
            }

            Validator<InUserBoard>.Valid(inputUserBoard, GetValidationConfigurations());

            if(inputUserBoard.BoardId != boardId) {
                throw new InconsistentRequestException(
                    $"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inputUserBoard.BoardId}"
                );
            }

            if(inputUserBoard.UserId != userId) {
                throw new InconsistentRequestException(
                    $"The user id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inputUserBoard.BoardId}"
                );
            }

            UserBoard user = await _usersBoardsRepository.FindAsync(boardId, userId);
            if(user == null) {
                throw new NotFoundException($"The UserBoard with board id {boardId} and user id {userId} not exists");
            }

            if(user.Permission != BoardPermission.Owner && inputUserBoard.Permission == InBoardPermission.Owner) {
                throw new InvalidChangeException("It is not possible change the permissions for owner.");
            }

            if(user.Permission == BoardPermission.Owner && inputUserBoard.Permission != InBoardPermission.Owner) {
                throw new InvalidChangeException("It is not possible change the permissions owner.");
            }

            user.In(inputUserBoard);

            await _usersBoardsRepository.UpdateAsync(user);

            return user.Out();
        }

        private static ValidatorConfiguration<InUserBoard> GetValidationConfigurations() {
            return new ValidatorConfiguration<InUserBoard>()
                .NotNull("BoardId", i => i.BoardId)
                .NotNull("UserId", i => i.UserId);
        }
    }
}
