using API.Domain;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services {
    class LineStyleService : ILineStyleService {
        private readonly ILineStyleRepository _lineStyleRepository;

        public LineStyleService(ILineStyleRepository lineStyleRepository) {
            _lineStyleRepository = lineStyleRepository;
        }

        public async Task CreateAsync(LineStyle lineStyle) {
            if(lineStyle == null) {
                throw new ArgumentNullException("Argument lineStyle can not be null");
            }

            await _lineStyleRepository.AddAsync(lineStyle);
        }

        public Task DeleteAsync(long id) {
            return _lineStyleRepository.RemoveAsync(id);
        }

        public Task<bool> ExistsAsync(long id) {
            return _lineStyleRepository.ExistsAsync(id);
        }

        public Task<IEnumerable<LineStyle>> GetAllAsync(long index, long size) {
            return _lineStyleRepository.GetAllAsync(index, size);
        }

        public Task<LineStyle> GetAsync(long id) {
            return _lineStyleRepository.FindAsync(id);
        }

        public Task UpdateAsync(LineStyle lineStyle) {
            if(lineStyle == null) {
                throw new ArgumentNullException("Argument lineStyle can not be null");
            }

            return _lineStyleRepository.UpdateAsync(lineStyle);
        }
    }
}
