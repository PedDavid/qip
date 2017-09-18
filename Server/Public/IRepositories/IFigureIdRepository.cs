using System.Threading.Tasks;

namespace QIP.Public.IRepositories {
    public interface IFigureIdRepository {
        Task<long> GetMaxIdAsync(long boardId);
    }
}
