using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IFiguresRepository {
        Task DeleteAsync(long boardId, long lastFigureToDelete = long.MaxValue);
    }
}
