using QIP.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QIP.Public.IRepositories {
    public interface IBoardRepository { 
        Task<bool> ExistsAsync(long id);
        Task AddAsync(Board board, string userId);
        Task<IEnumerable<Board>> GetAllAsync(long index, long size);
        Task<IEnumerable<Board>> GetAllAsync(long index, long size, string search);
        Task<Board> FindAsync(long id);
        Task RemoveAsync(long id);
        Task UpdateAsync(Board board);
    }
}
