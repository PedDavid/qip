using API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IBoardRepository { 
        Task<bool> ExistsAsync(long id);
        Task<long> AddAsync(Board board, string userId);
        Task<IEnumerable<Board>> GetAllAsync(long index, long size);
        Task<IEnumerable<Board>> GetAllAsync(long index, long size, string search);
        Task<Board> FindAsync(long id);
        Task RemoveAsync(long id);
        Task UpdateAsync(Board board);
        Task PartialUpdateAsync(Board board);
    }
}
