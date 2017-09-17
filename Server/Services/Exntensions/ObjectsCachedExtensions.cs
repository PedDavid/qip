using QIP.Domain;
using QIP.Public.IRepositories;
using QIP.Services.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace QIP.Services.Exntensions {
    public static class ObjectsCachedExtensions {
        public static async Task<AccessToken> GetAccessToken(this IMemoryCache cache, IAuth0ManagementRepository auth0ManagementRepository) {
            return await cache.GetOrCreateAsync(
                "access_token",
                CacheRegions.AUTH0,
                async cacheEntry => {
                    AccessToken token = await auth0ManagementRepository.GetAccessToken();

                    cacheEntry.Priority = CacheItemPriority.NeverRemove;
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token.Expires_in) - TimeSpan.FromSeconds(30);

                    return token;
                }
            );
        }
    }
}
