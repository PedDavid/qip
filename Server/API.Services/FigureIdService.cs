using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using API.Services.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services {
    public class FigureIdService : IFigureIdService {
        private readonly IFigureIdRepository _figureIdRepository;
        private readonly IMemoryCache _memoryCache;

        public FigureIdService(IFigureIdRepository figureIdRepository, IMemoryCache memoryCache) {
            _figureIdRepository = figureIdRepository;
            _memoryCache = memoryCache;
        }

        public Task<IFigureIdGenerator> GetOrCreateFigureIdGeneratorAsync(long boardId) {
            return _memoryCache.GetOrCreateAsync(
                boardId.ToString(),
                CacheRegions.FIGURE_ID_GENERATOR,
                async cacheEntry => {
                    long initialId = await _figureIdRepository.GetMaxIdAsync(boardId);

                    cacheEntry.Priority = CacheItemPriority.High;
                    cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(90);//TODO REVER, FALAR COM O GAMBOA

                    return (IFigureIdGenerator) new FigureIdGenerator(initialId);
                }
            );
        }

        private class FigureIdGenerator : IFigureIdGenerator {
            private long _id;

            public FigureIdGenerator(long initId) {
                _id = initId;
            }

            public long NewId() {
                return Interlocked.Increment(ref _id);
            }
        }
    }
}
