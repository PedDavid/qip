using API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IUsersBoardsRepository {
        Task Add(UserBoard userBoard);

        Task<UserBoard> Find(long boardId, long userId);

        Task<UserBoard_Board> FindBoard(long userId, long boardId);

        Task<UserBoard_User> FindUser(long boardId, long userId);

        Task<IEnumerable<UserBoard_Board>> GetAllBoards(long userId);

        Task<IEnumerable<UserBoard_User>> GetAllUsers(long boardId);

        Task Remove(long boardId, long userId);

        Task Update(UserBoard userBoard);
    }
}
