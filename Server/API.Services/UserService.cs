using API.Domain;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using API.Services.Exntensions;
using IODomain.Extensions;
using IODomain.Output;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services {
    public class UserService : IUserService {
        private readonly IAuth0ManagementRepository _auth0ManagementRepository;
        private readonly IMemoryCache _memoryCache;

        public UserService(IAuth0ManagementRepository auth0ManagementRepository, IMemoryCache memoryCache) {
            _auth0ManagementRepository = auth0ManagementRepository;
            _memoryCache = memoryCache;
        }

        public async Task<bool> ExistsAsync(string userId) {
            AccessToken accessToken = await _memoryCache.GetAccessToken(_auth0ManagementRepository);

            return await _auth0ManagementRepository.UserExistsAsync(userId, accessToken.Access_token);
        }

        public async Task<IEnumerable<User>> GetAllAsync(long index, long size, string search) {
            AccessToken accessToken = await _memoryCache.GetAccessToken(_auth0ManagementRepository);

            return search == null ?
                 await _auth0ManagementRepository.GetUsersAsync(accessToken.Access_token, index, size) :
                 await _auth0ManagementRepository.GetUsersAsync(accessToken.Access_token, index, size, search);
        }

        public Task<IEnumerable<User>> GetAllAsync(long index, long size) {
            return GetAllAsync(index, size, null);
        }

        public async Task<User> GetAsync(string userId) {
            AccessToken accessToken = await _memoryCache.GetAccessToken(_auth0ManagementRepository);

            return await _auth0ManagementRepository.GetUserAsync(userId, accessToken.Access_token);
        }
    }
}
