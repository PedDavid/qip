using QIP.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QIP.Public.IServices {
    public interface IUserService {
        Task<IEnumerable<User>> GetAllAsync(long index, long size);

        Task<IEnumerable<User>> GetAllAsync(long index, long size, string search);

        Task<User> GetAsync(string userId); 

        Task<bool> ExistsAsync(string userId);
    }
}
