using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Extensions {
    public static class IMemoryCacheExtensions {
        public static Task<TItem> GetOrCreateAsync<TItem>(this IMemoryCache cache, string key, string region, Func<ICacheEntry, Task<TItem>> factory) {
            return cache.GetOrCreateAsync(CreateKeyWithRegion(key, region), factory);
        }

        private static string CreateKeyWithRegion(string key, string region) {//TODO ALTERAR
            return "region:" + (region ?? "null_region") + ";key=" + key;
        }
    }
}
