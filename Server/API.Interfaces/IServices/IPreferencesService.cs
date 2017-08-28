using IODomain.Input;
using IODomain.Output;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IPreferencesService {
        Task<OutPreferences> GetAsync(string userId);

        //This method only return OutPreferences if it is created, otherwise return null
        Task<OutPreferences> CreateOrUpdateAsync(string userId, InPreferences inPreferences);

        Task DeleteAsync(string userId);
    }
}
