using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IDefaultRepository<T> {
        Task<long> AddAsync(T t);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindAsync(long id);
        Task RemoveAsync(long id);
        Task UpdateAsync(T t);
        Task PartialUpdateAsync(T t);
    }
}
