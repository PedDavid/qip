using System.Threading.Tasks;

namespace QIP.Public.IServices {
    public interface IFiguresService {
        Task DeleteAsync(long boardId, long lastFigureToDelete = long.MaxValue);
    }
}
