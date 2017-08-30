using API.Domain;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using API.Interfaces.ServicesExceptions;
using API.Services.Utils;
using IODomain.Extensions;
using IODomain.Input;
using IODomain.Output;
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

        public async Task<OutLine> CreateAsync(long boardId, InLine inLine) {
            if(inLine == null) {
                throw new MissingInputException();
            }

            Validator<InLine>.Valid(inLine, GetCreateValidationConfigurations());

            if(inLine.BoardId != boardId) {
                throw new InconsistentRequestException(
                    $"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inLine.BoardId}"
                );
            }

            if(!await _boardRepository.ExistsAsync(boardId)) {
                throw new NotFoundException($"The Board with id {boardId} not exists");
            }

            IFigureIdGenerator idGen = await _figureIdService.GetOrCreateFigureIdGeneratorAsync(boardId);

            Line line = new Line(boardId, idGen.NewId()).In(inLine);
            long id = await _lineRepository.AddAsync(line);

            return line.Out();
        }

        private static ValidatorConfiguration<InLine> GetCreateValidationConfigurations() {
            var pointValidationConfig = new ValidatorConfiguration<InLinePoint>()
                .NotNull("X", p => p.X)
                .NotNull("Y", p => p.Y)
                .NotNull("Style", p => p.Style)
                .NotNull("Style.Width", p => p.Style.Width);

            return new ValidatorConfiguration<InLine>()
                .NotNull("BoardId", i => i.BoardId)
                .NotNull("Style", i => i.Style)
                //.NotNull("Style.Id", i => i.Style.Id)
                .NotNull("Style.Color", i => i.Style.Color)
                .UseValidator("Points", i => i.Points, pointValidationConfig);
        }

        public async Task DeleteAsync(long id, long boardId) {
            if(!await _lineRepository.ExistsAsync(id, boardId)) {
                throw new NotFoundException($"The line with id {id}, belonging to board with id {boardId}, does not exist");
            }

            await _lineRepository.RemoveAsync(id, boardId);
        }

        public async Task<IEnumerable<OutLine>> GetAllAsync(long boardId) {
            if(!await _boardRepository.ExistsAsync(boardId)) {
                throw new NotFoundException($"The Board with id {boardId} not exists");
            }

            IEnumerable<Line> lines = await _lineRepository.GetAllAsync(boardId);

            return lines.Select(LineExtensions.Out);
        }

        public async Task<OutLine> GetAsync(long id, long boardId) {
            Line line = await _lineRepository.FindAsync(id, boardId);

            if(line == null) {
                throw new NotFoundException($"The line with id {id}, belonging to board with id {boardId}, does not exist");
            }

            return line.Out();
        }

        public async Task<OutLine> UpdateAsync(long id, long boardId, InLine inLine) {
            if(inLine == null) {
                throw new MissingInputException();
            }

            Validator<InLine>.Valid(inLine, GetUpdateValidationConfigurations());

            if(inLine.Id != id) {
                throw new InconsistentRequestException(
                    $"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inLine.Id}"
                );
            }

            if(inLine.BoardId != boardId) {
                throw new InconsistentRequestException(
                    $"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inLine.BoardId}"
                );
            }

            Line line = await _lineRepository.FindAsync(id, boardId);
            if(line == null) {
                throw new NotFoundException($"The line with id {id}, belonging to board with id {boardId}, does not exist");
            }

            line.In(inLine);

            await _lineRepository.UpdateAsync(line);

            return line.Out();
        }

        private static ValidatorConfiguration<InLine> GetUpdateValidationConfigurations() {
            return GetCreateValidationConfigurations()
                .NotNull("Id", i => i.Id);
        }
    }
}
