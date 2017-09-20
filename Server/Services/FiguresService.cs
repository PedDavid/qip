using QIP.Public.IRepositories;
using QIP.Public.IServices;
using QIP.Public.ServicesExceptions;
using System.Threading.Tasks;

namespace QIP.Services {
    public class FiguresService : IFiguresService {
        private readonly IFiguresRepository _figuresRepository;
        private readonly IBoardService _boardRepository;

        public FiguresService(IFiguresRepository figuresRepository, IBoardService boardRepository) {
            _figuresRepository = figuresRepository;
            _boardRepository = boardRepository;
        }
        
        public async Task DeleteAsync(long boardId, long lastFigureToDelete = long.MaxValue) {
            if(!await _boardRepository.ExistsAsync(boardId)) {
                throw new NotFoundException($"The Board with id {boardId} not exists");
            }

            await _figuresRepository.DeleteAsync(boardId, lastFigureToDelete);
        }
    }
}
