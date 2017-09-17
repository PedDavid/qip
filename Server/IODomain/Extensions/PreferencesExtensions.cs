using QIP.Domain;
using QIP.IODomain.Input;
using QIP.IODomain.Output;

namespace QIP.IODomain.Extensions {
    public static class PreferencesExtensions {
        public static OutPreferences Out(this Preferences preferences) {
            return new OutPreferences() {
                UserId = preferences.UserId,
                Favorites = preferences.Favorites,
                PenColors = preferences.PenColors,
                DefaultPen = preferences.DefaultPen,
                DefaultEraser = preferences.DefaultEraser,
                CurrTool = preferences.CurrTool,
                Settings = preferences.Settings
            };
        }

        public static Preferences In(this Preferences preferences, InPreferences inPreferences) {
            preferences.Favorites = inPreferences.Favorites;
            preferences.PenColors = inPreferences.PenColors;
            preferences.DefaultPen = inPreferences.DefaultPen;
            preferences.DefaultEraser = inPreferences.DefaultEraser;
            preferences.CurrTool = inPreferences.CurrTool;
            preferences.Settings = inPreferences.Settings;

            return preferences;
        }
    }
}
