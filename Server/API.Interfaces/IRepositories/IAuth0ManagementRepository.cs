using API.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IAuth0ManagementRepository {
        Task<AccessToken> GetAccessToken();
        Task<List<User>> GetUsersAsync(string access_token, long index, long size, string search);
        Task<List<User>> GetUsersAsync(string access_token, long index, long size);
        Task<bool> UserExistsAsync(string userId, string access_token);
    }
}
