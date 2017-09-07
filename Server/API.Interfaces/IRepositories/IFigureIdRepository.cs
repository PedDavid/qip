using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IFigureIdRepository {
        Task<long> GetMaxIdAsync(long boardId);
    }
}
