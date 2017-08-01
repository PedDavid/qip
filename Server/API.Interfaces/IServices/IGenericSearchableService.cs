using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IGenericSearchableService<I,O> : IGenericService<I, O> {
        Task<IEnumerable<O>> GetAllAsync(string search, long index, long size);
    }
}
