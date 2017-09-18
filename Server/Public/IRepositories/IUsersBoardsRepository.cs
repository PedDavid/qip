using QIP.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QIP.Public.IRepositories {
    public interface IUsersBoardsRepository {
        Task<bool> ExistsAsync(long boardId, string userId);

        Task AddAsync(UserBoard userBoard);

        Task<UserBoard> FindAsync(long boardId, string userId);

        Task<BoardPermission> FindPermissionAsync(string userId, long boardId);

        Task<UserBoard_Board> FindBoardAsync(string userId, long boardId);

        Task<UserBoard_User> FindUserAsync(long boardId, string userId);

        Task<IEnumerable<UserBoard_Board>> GetAllBoardsAsync(string userId, long index, long size);

        Task<IEnumerable<UserBoard_Board>> GetAllBoardsAsync(string userId, long index, long size, string search);

        Task<IEnumerable<UserBoard_User>> GetAllUsersAsync(long boardId, long index, long size);

        Task<IEnumerable<UserBoard_User>> GetAllUsersAsync(long boardId, long index, long size, string search);

        Task RemoveAsync(long boardId, string userId);

        Task UpdateAsync(UserBoard userBoard);
    }
}
