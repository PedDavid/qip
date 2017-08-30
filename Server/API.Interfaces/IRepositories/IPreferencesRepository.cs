using API.Domain;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IPreferencesRepository {
        Task<Preferences> FindAsync(string userId);
        Task<bool> AddOrUpdateAsync(string userId, Preferences preferences);
        Task RemoveAsync(string userId);
    }
}
