using API.Interfaces.IRepositories;
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

        public static Task<FigureIdGenerator> GetFigureIdGenerator(this IMemoryCache memoryCache, IFigureIdRepository figureIdRepository, long clientId) {
            return memoryCache.GetOrCreateAsync(
                clientId.ToString(),
                CacheRegions.FIGURE_ID_GENERATOR,
                async cacheEntry => {
                    FigureIdGenerator idGen = await FigureIdGenerator.Create(figureIdRepository);

                    cacheEntry.Priority = CacheItemPriority.High;
                    cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(90);//TODO REVER, FALAR COM O GAMBOA

                    return idGen;
                }
            );
        }
    }
}
