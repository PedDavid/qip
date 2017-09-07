﻿using API.Domain;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using API.Interfaces.ServicesExceptions;
using IODomain.Extensions;
using IODomain.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services {
    class LineService : ILineService {
        private readonly ILineRepository _lineRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly IFigureIdService _figureIdService;

        public LineService(ILineRepository lineRepository, IBoardRepository boardRepository, IFigureIdService figureIdService) {
            _lineRepository = lineRepository;
            _boardRepository = boardRepository;
            _figureIdService = figureIdService;
        }

        public async Task CreateAsync(Line line) {
            if(line == null) {
                throw new ArgumentNullException("Argument line can not be null");
            }

            if(!await _boardRepository.ExistsAsync(line.BoardId)) {
                throw new NotFoundException($"The Board with id {line.BoardId} not exists");
            }

            IFigureIdGenerator idGen = await _figureIdService.GetOrCreateFigureIdGeneratorAsync(line.BoardId);

            line.Id = idGen.NewId();

            await _lineRepository.AddAsync(line);
        }

        public Task DeleteAsync(long id, long boardId) {
            return _lineRepository.RemoveAsync(id, boardId);
        }

        public Task<bool> ExistsAsync(long id, long boardId) {
            return _lineRepository.ExistsAsync(id, boardId);
        }

        public async Task<IEnumerable<Line>> GetAllAsync(long boardId) {
            if(!await _boardRepository.ExistsAsync(boardId)) {
                throw new NotFoundException($"The Board with id {boardId} not exists");
            }

            return await _lineRepository.GetAllAsync(boardId);
        }

        public Task<Line> GetAsync(long id, long boardId) {
            return _lineRepository.FindAsync(id, boardId);
        }

        public Task UpdateAsync(Line line) {
            if(line == null) {
                throw new ArgumentNullException("Argument line can not be null");
            }

            return _lineRepository.UpdateAsync(line);
        }
    }
}
