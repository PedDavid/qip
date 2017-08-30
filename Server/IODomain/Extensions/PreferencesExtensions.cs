using API.Domain;
using IODomain.Input;
using IODomain.Output;

namespace IODomain.Extensions {
    public static class PreferencesExtensions {
        public static OutPreferences Out(this Preferences preferences) {
            return new OutPreferences() {
                UserId = preferences.UserId,
                Favorites = preferences.Favorites,
                PenColors = preferences.PenColors
            };
        }

        public static Preferences In(this Preferences preferences, InPreferences inPreferences) {
            preferences.Favorites = inPreferences.Favorites;
            preferences.PenColors = inPreferences.PenColors;

            return preferences;
        }
    }
}
