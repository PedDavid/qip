using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IGenericSearchableService<T> : IGenericService<T> {
        Task<IEnumerable<T>> GetAllAsync(long index, long size, string search);
    }
}
