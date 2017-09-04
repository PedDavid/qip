using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IFigureRepository<T> {
        Task<bool> ExistsAsync(long id, long boardId);
        Task<long> AddAsync(T figure);
        Task<IEnumerable<T>> GetAllAsync(long boardId);
        Task<T> FindAsync(long id, long boardId);
        Task RemoveAsync(long id, long boardId);
        Task UpdateAsync(T figure);
    }
}
