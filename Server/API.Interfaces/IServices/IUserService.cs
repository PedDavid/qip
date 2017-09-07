using API.Domain;
using IODomain.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IUserService {
        Task<IEnumerable<User>> GetAllAsync(long index, long size);

        Task<IEnumerable<User>> GetAllAsync(long index, long size, string search);

        Task<User> GetAsync(string userId); 

        Task<bool> ExistsAsync(string userId);
    }
}
