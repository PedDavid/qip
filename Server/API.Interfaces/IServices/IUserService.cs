using IODomain.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IUserService {
        Task<IEnumerable<OutUser>> GetAllAsync(long index, long size);

        Task<IEnumerable<OutUser>> GetAllAsync(long index, long size, string search);
    }
}
