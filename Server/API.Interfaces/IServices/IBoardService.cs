using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IBoardService {
        Task<IEnumerable<OutBoard>> GetAllAsync(long index, long size);

        Task<IEnumerable<OutBoard>> GetAllAsync(long index, long size, string search);

        Task<OutBoard> GetAsync(long id);

        Task<OutBoard> CreateAsync(InBoard inBoard, string userId);

        Task<OutBoard> UpdateAsync(long id, InBoard inBoard);

        Task DeleteAsync(long id);
    }
}
