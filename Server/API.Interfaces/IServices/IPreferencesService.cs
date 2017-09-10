using API.Domain;
using IODomain.Input;
using IODomain.Output;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IPreferencesService {
        Task<Preferences> GetAsync(string userId);

        //return: true if preferences is created
        Task<bool> CreateOrUpdateAsync(Preferences preferences);

        Task DeleteAsync(string userId);

        Task<bool> ExistsAsync(string userId);
    }
}
