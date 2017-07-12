using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IDefaultRepository<T> {
        Task<long> Add(T t);
        Task<IEnumerable<T>> GetAll();
        Task<T> Find(long id);
        Task Remove(long id);
        Task Update(T t);
        Task PartialUpdate(T t);
    }
}
