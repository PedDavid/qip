using System.Threading.Tasks;

namespace QIP.Public.IRepositories {
    public interface IFiguresRepository {
        Task DeleteAsync(long boardId, long lastFigureToDelete = long.MaxValue);
    }
}
