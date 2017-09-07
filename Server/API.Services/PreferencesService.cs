using API.Domain;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using API.Interfaces.ServicesExceptions;
using System;
using System.Threading.Tasks;

namespace API.Services {
    class PreferencesService : IPreferencesService {
        private readonly IPreferencesRepository _preferencesRepository;
        private readonly IUserService _userService;

        public PreferencesService(IPreferencesRepository preferencesRepository, IUserService userService) {
            _preferencesRepository = preferencesRepository;
            _userService = userService;
        }

        public async Task<bool> CreateOrUpdateAsync(Preferences preferences) {
            if(preferences == null) {
                throw new ArgumentNullException("Argument preferences can not be null");
            }

            if(!await _userService.ExistsAsync(preferences.UserId)) {
                throw new NotFoundException($"The User with id {preferences.UserId} not exists");
            }

            return await _preferencesRepository.AddOrUpdateAsync(preferences);
        }

        public Task DeleteAsync(string userId) {
            return _preferencesRepository.RemoveAsync(userId);
        }

        public Task<bool> ExistsAsync(string userId) {
            return _preferencesRepository.ExistsAsync(userId);
        }

        public Task<Preferences> GetAsync(string userId) {
            return _preferencesRepository.FindAsync(userId);
        }
    }
}
