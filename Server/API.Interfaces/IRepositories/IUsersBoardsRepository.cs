using API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IUsersBoardsRepository {
        Task AddAsync(UserBoard userBoard);

        Task<UserBoard> FindAsync(long boardId, long userId);

        Task<UserBoard_Board> FindBoardAsync(long userId, long boardId);

        Task<UserBoard_User> FindUserAsync(long boardId, long userId);

        Task<IEnumerable<UserBoard_Board>> GetAllBoardsAsync(long userId);

        Task<IEnumerable<UserBoard_User>> GetAllUsersAsync(long boardId);

        Task RemoveAsync(long boardId, long userId);

        Task UpdateAsync(UserBoard userBoard);
    }
}
