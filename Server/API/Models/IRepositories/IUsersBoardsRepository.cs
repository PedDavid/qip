using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.IRepositories {
    public interface IUsersBoardsRepository {
        void Add(UserBoard userBoard);

        UserBoard Find(long boardId, long userId);

        UserBoard_Board FindBoard(long userId, long boardId);

        UserBoard_User FindUser(long boardId, long userId);

        IEnumerable<UserBoard_Board> GetAllBoards(long userId);

        IEnumerable<UserBoard_User> GetAllUsers(long boardId);

        void Remove(long boardId, long userId);

        void Update(UserBoard userBoard);
    }
}
