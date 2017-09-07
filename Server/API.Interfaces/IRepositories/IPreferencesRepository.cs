using API.Domain;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IPreferencesRepository {
        Task<bool> ExistsAsync(string userId);
        Task<Preferences> FindAsync(string userId);
        Task<bool> AddOrUpdateAsync(Preferences preferences);
        Task RemoveAsync(string userId);
    }
}
