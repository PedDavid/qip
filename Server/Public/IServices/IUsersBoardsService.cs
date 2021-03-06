﻿using QIP.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QIP.Public.IServices {
    public interface IUsersBoardsService {
        Task<IEnumerable<UserBoard_User>> GetAllUsersAsync(long boardId, long index, long size, string search);

        Task<UserBoard_User> GetUserAsync(long boardId, string userId);

        Task CreateAsync(UserBoard userBoard);

        Task UpdateAsync(UserBoard userBoard);

        Task DeleteAsync(long boardId, string userId);

        Task<IEnumerable<UserBoard_Board>> GetAllBoardsAsync(string userId, long index, long size, string search);

        Task<UserBoard_Board> GetBoardAsync(string userId, long boardId);

        Task<UserBoard> GetAsync(string userId, long boardId);

        Task<BoardPermission> GetPermissionAsync(string userId, long boardId);

        Task<bool> ExistsAsync(string userId, long boardId);

    }
}
