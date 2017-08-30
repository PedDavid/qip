using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IDefaultRepository<T> {
        Task<bool> ExistsAsync(long id);
        Task<long> AddAsync(T t);
        Task<IEnumerable<T>> GetAllAsync(long index, long size);
        Task<T> FindAsync(long id);
        Task RemoveAsync(long id);
        Task UpdateAsync(T t);
        Task PartialUpdateAsync(T t);
    }
}
