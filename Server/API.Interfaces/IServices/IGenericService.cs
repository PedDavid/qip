using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IGenericService<I, O> {
        Task<IEnumerable<O>> GetAllAsync(long index, long size);

        Task<O> GetAsync(long id);

        Task<O> CreateAsync(I input);

        Task<O> UpdateAsync(long id, I input);

        Task DeleteAsync(long id);
    }
}
