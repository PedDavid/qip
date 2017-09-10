using API.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IBoardService {
        Task<bool> ExistsAsync(long id);

        Task<IEnumerable<Board>> GetAllAsync(long index, long size);

        Task<IEnumerable<Board>> GetAllAsync(long index, long size, string search = null);

        Task<Board> GetAsync(long id);

        Task CreateAsync(Board board, string userId);

        Task UpdateAsync(Board board);

        Task DeleteAsync(long id);
    }
}
