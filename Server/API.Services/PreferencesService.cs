using API.Domain;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using IODomain.Extensions;
using IODomain.Input;
using IODomain.Output;
using System.Threading.Tasks;

namespace API.Services {
    class PreferencesService : IPreferencesService {//TODO ADD CHECKS
        private readonly IPreferencesRepository _preferencesRepository;

        public PreferencesService(IPreferencesRepository preferencesRepository) {
            _preferencesRepository = preferencesRepository;
        }

        public async Task<OutPreferences> CreateOrUpdateAsync(string userId, InPreferences inPreferences) {
            Preferences preferences = new Preferences(userId).In(inPreferences);

            bool created = await _preferencesRepository.AddOrUpdateAsync(userId, preferences);

            return created ? preferences.Out() : null;
        }

        public Task DeleteAsync(string userId) {
            return _preferencesRepository.RemoveAsync(userId);
        }

        public async Task<OutPreferences> GetAsync(string userId) {
            Preferences preferences = await _preferencesRepository.FindAsync(userId);
            return preferences.Out();
        }
    }
}
