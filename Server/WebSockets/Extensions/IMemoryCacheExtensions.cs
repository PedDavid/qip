using API.Interfaces.IRepositories;
using API.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace WebSockets.Extensions {
    public static class IMemoryCacheExtensions {
        private static string FIGURE_ID_GENERATOR_REGION = "FIGURE_ID";//TODO REVER HARD CODED NESTA CLASS-> REGIÂO USADA POR VARIOS PROJECTOS

        public static Task<FigureIdGenerator> GetFigureIdGenerator(this IMemoryCache memoryCache, IFigureIdRepository figureIdRepository, long clientId) {
            return memoryCache.GetOrCreateAsync(
                clientId.ToString(),
                FIGURE_ID_GENERATOR_REGION,
                async cacheEntry => {
                    FigureIdGenerator idGen = await FigureIdGenerator.Create(figureIdRepository);

                    //TODO CONFIG CACHE ENTRY
                    //AbsoluteExpiration -> Não faz sentido
                    //SlidingExpiration -> ??Defenir um tempo??
                    //Priority -> NeverRemove não faz sentido
                    //Priority -> ??CacheItemPriority.High??
                    //ExpirationTokens -> Não vejo razão para tal

                    return idGen;
                }
            );
        }
    }
}
