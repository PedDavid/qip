using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IGenericService<T> {
        Task<IEnumerable<T>> GetAllAsync(long index, long size);

        Task<T> GetAsync(long id);

        Task CreateAsync(T input);

        Task UpdateAsync(T input);

        Task DeleteAsync(long id);

        Task<bool> ExistsAsync(long id);
    }
}
