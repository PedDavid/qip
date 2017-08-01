using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IUsersBoardsService {
        Task<IEnumerable<OutUserBoard_User>> GetAllUsersAsync(long boardId, string search, long index, long size);

        Task<OutUserBoard_User> GetUserAsync(long boardId, long userId);

        Task<OutUserBoard> CreateAsync(long boardId, InUserBoard inputUserBoard);

        Task<OutUserBoard> UpdateAsync(long boardId, long userId, InUserBoard inputUserBoard);

        Task DeleteAsync(long boardId, long userId);

        Task<IEnumerable<OutUserBoard_Board>> GetAllBoardsAsync(long userId, string search, long index, long size);

        Task<OutUserBoard_Board> GetBoardAsync(long userId, long boardId);

    }
}
