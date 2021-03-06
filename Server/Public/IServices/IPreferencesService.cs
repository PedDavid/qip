﻿using QIP.Domain;
using System.Threading.Tasks;

namespace QIP.Public.IServices {
    public interface IPreferencesService {
        Task<Preferences> GetAsync(string userId);

        //return: true if preferences is created
        Task<bool> CreateOrUpdateAsync(Preferences preferences);

        Task DeleteAsync(string userId);

        Task<bool> ExistsAsync(string userId);
    }
}
