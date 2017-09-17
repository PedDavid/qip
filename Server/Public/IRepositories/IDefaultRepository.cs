using System.Collections.Generic;
using System.Threading.Tasks;

namespace QIP.Public.IRepositories {
    public interface IDefaultRepository<T> {
        Task<bool> ExistsAsync(long id);
        Task AddAsync(T t);
        Task<IEnumerable<T>> GetAllAsync(long index, long size);
        Task<T> FindAsync(long id);
        Task RemoveAsync(long id);
        Task UpdateAsync(T t);
    }
}
