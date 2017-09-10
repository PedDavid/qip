using API.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IAuth0ManagementRepository {
        Task<AccessToken> GetAccessToken();
        Task<IEnumerable<User>> GetUsersAsync(string access_token, long index, long size, string search);
        Task<IEnumerable<User>> GetUsersAsync(string access_token, long index, long size);
        Task<User> GetUserAsync(string userId, string access_token);
        Task<bool> UserExistsAsync(string userId, string access_token);
    }
}
