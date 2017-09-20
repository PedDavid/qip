using System.Collections.Generic;
using System.Threading.Tasks;

namespace QIP.Public.IRepositories {
    public interface IFigureRepository<T> {
        Task<bool> ExistsAsync(long id, long boardId);
        Task AddAsync(T figure);
        Task<IEnumerable<T>> GetAllAsync(long boardId);
        Task<T> FindAsync(long id, long boardId);
        Task RemoveAsync(long id, long boardId);
        Task UpdateAsync(T figure);
    }
}
