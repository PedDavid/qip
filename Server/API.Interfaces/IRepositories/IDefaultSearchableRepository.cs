using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IDefaultSearchableRepository<T> : IDefaultRepository<T> {
        Task<IEnumerable<T>> GetAllAsync(string search, long index, long size);
    }
}
