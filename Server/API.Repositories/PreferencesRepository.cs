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

        public async Task<bool> AddOrUpdateAsync(Preferences preferences) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@userId", SqlDbType.VarChar)
                    .Value = preferences.UserId;

            parameters
                .Add("@favorites", SqlDbType.VarChar)
                .Value = preferences.Favorites ?? SqlString.Null;

            parameters
                .Add("@penColors", SqlDbType.VarChar)
                .Value = preferences.PenColors ?? SqlString.Null;

            parameters
                .Add("@defaultPen", SqlDbType.VarChar)
                .Value = preferences.DefaultPen ?? SqlString.Null;

            parameters
                .Add("@defaultEraser", SqlDbType.VarChar)
                .Value = preferences.DefaultEraser ?? SqlString.Null;

            parameters
                .Add("@currTool", SqlDbType.VarChar)
                .Value = preferences.CurrTool ?? SqlString.Null;

            parameters
                .Add("@settings", SqlDbType.VarChar)
                .Value = preferences.Settings ?? SqlString.Null;

            SqlParameter outParam = parameters.Add("@created", SqlDbType.Bit);
            outParam.Direction = ParameterDirection.Output;

            await _queryTemplate.StoredProcedureAsync(INSERT_UPDATE_PREFERENCES, parameters);

            return (bool)outParam.Value;
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

        public Task<bool> ExistsAsync(string userId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.VarChar).Value = userId;

            return _queryTemplate.QueryForScalarAsync<bool>(PREFERENCES_EXISTS, parameters);
        }

        //SQL Commands
        private static readonly string PREFERENCES_EXISTS = "SELECT CAST(count(id) as BIT) FROM dbo.Preferences WHERE id = @id";
        private static readonly string SELECT_PREFERENCES = "SELECT id, favorites, penColors, defaultPen, defaultEraser, currTool, settings " +
                                                            "FROM dbo.Preferences " +
                                                            "WHERE id = @id";
        private static readonly string DELETE_USER = "DELETE FROM dbo.Preferences WHERE id = @id";

        //SQL Stored Procedures
        private static readonly string INSERT_UPDATE_PREFERENCES = "dbo.InsertOrUpdatePreferences";

        //Extract Data From Data Reader
        private static Preferences GetPreferences(SqlDataReader dr) {
            return new Preferences() {
                UserId = dr.GetString(0),
                Favorites = dr.GetString(1),
                PenColors = dr.GetString(2),
                DefaultPen = dr.GetString(3),
                DefaultEraser = dr.GetString(4),
                CurrTool = dr.GetString(5),
                Settings = dr.GetString(6)
            };
        }
    }
}
