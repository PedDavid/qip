using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using API.Interfaces.IRepositories;
using API.Domain;

namespace API.Repositories {
    public class PreferencesRepository : IPreferencesRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public PreferencesRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public async Task<bool> AddOrUpdateAsync(string userId, Preferences preferences) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@userId", SqlDbType.VarChar)
                    .Value = userId;

            parameters
                .Add("@favorites", SqlDbType.VarChar)
                .Value = preferences.Favorites;

            parameters
                .Add("@penColors", SqlDbType.VarChar)
                .Value = preferences.PenColors;

            SqlParameter outParam = parameters.Add("@created", SqlDbType.Bit);
            outParam.Direction = ParameterDirection.Output;

            await _queryTemplate.StoredProcedureAsync(INSERT_UPDATE_PREFERENCES, parameters);

            return (bool) outParam.Value;
        }

        public Task<Preferences> FindAsync(string userId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.VarChar).Value = userId;

            return _queryTemplate.QueryForObjectAsync(SELECT_PREFERENCES, parameters, GetPreferences);
        }

        public Task RemoveAsync(string userId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.VarChar).Value = userId;

            return _queryTemplate.CommandAsync(DELETE_USER, parameters);
        }

        //SQL Commands
        private static readonly string SELECT_PREFERENCES = "SELECT id, favorites, penColors FROM dbo.Preferences WHERE id = @id";
        private static readonly string DELETE_USER = "DELETE FROM dbo.Preferences WHERE id = @id";

        //SQL Stored Procedures
        private static readonly string INSERT_UPDATE_PREFERENCES = "dbo.InsertOrUpdatePreferences";

        //Extract Data From Data Reader
        private static Preferences GetPreferences(SqlDataReader dr) {
            return new Preferences(dr.GetString(0)) {
                Favorites = dr.GetString(1),
                PenColors = dr.GetString(2)
            };
        }
    }
}
