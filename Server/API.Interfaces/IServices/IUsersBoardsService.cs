using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IUsersBoardsService {
        Task<IEnumerable<OutUserBoard_User>> GetAllUsersAsync(long boardId, long index, long size, string search);

        Task<OutUserBoard_User> GetUserAsync(long boardId, string userId);

        Task<OutUserBoard> CreateAsync(long boardId, InUserBoard inputUserBoard);

        Task<OutUserBoard> UpdateAsync(long boardId, string userId, InUserBoard inputUserBoard);

        Task DeleteAsync(long boardId, string userId);

        Task<IEnumerable<OutUserBoard_Board>> GetAllBoardsAsync(string userId, long index, long size, string search);

        Task<OutUserBoard_Board> GetBoardAsync(string userId, long boardId);

        Task<OutBoardPermission> GetPermissionAsync(string userId, long boardId);

    }
}
