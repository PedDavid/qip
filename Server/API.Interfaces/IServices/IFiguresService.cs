using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IFiguresService {
        Task DeleteAsync(long boardId, long lastFigureToDelete = long.MaxValue);
    }
}
