using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace API.Repositories {
    public class SqlServerTemplate {
        private readonly DatabaseOptions _options;

        public SqlServerTemplate(IOptionsSnapshot<DatabaseOptions> options) {
            _options = options.Value;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, List<SqlParameter> parameters, Func<SqlDataReader, T> rowMapper) {
            List<T> result = new List<T>();
            using(SqlConnection con = new SqlConnection(_options.Context)) {
                using(SqlCommand cmd = con.CreateCommand()) {
                    cmd.CommandText = sql;

                    parameters.ForEach(prm => cmd.Parameters.Add(prm));

                    await con.OpenAsync();
                    SqlDataReader dr = await cmd.ExecuteReaderAsync();

                    while(await dr.ReadAsync())
                        result.Add(rowMapper(dr));

                    return result;
                }
            }
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, Func<SqlDataReader, T> rowMapper) {
            return QueryAsync(sql, new List<SqlParameter>(), rowMapper);
        }

        public async Task CommandAsync(string sql, List<SqlParameter> parameters) {
            using(SqlConnection con = new SqlConnection(_options.Context)) {
                using(SqlCommand cmd = con.CreateCommand()) {
                    cmd.CommandText = sql;

                    parameters.ForEach(prm => cmd.Parameters.Add(prm));

                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public Task CommandAsync(string sql) {
            return CommandAsync(sql, new List<SqlParameter>());
        }

        public async Task<T> QueryForScalarAsync<T>(string sql, List<SqlParameter> parameters, bool defaultToLackValue = false) {
            using(SqlConnection con = new SqlConnection(_options.Context)) {
                using(SqlCommand cmd = con.CreateCommand()) {
                    cmd.CommandText = sql;

                    parameters.ForEach(prm => cmd.Parameters.Add(prm));

                    await con.OpenAsync();

                    object scalar = await cmd.ExecuteScalarAsync();

                    if(defaultToLackValue && scalar == null)
                        return default(T);

                    return (T) scalar;
                }
            }
        }

        public Task<T> QueryForScalarAsync<T>(string sql) {
            return QueryForScalarAsync<T>(sql, new List<SqlParameter>());
        }

        public async Task<T> QueryForObjectAsync<T>(string sql, List<SqlParameter> parameters, Func<SqlDataReader, T> rowMapper) where T : class {
            using(SqlConnection con = new SqlConnection(_options.Context)) {
                using(SqlCommand cmd = con.CreateCommand()) {
                    cmd.CommandText = sql;

                    parameters.ForEach(prm => cmd.Parameters.Add(prm));

                    await con.OpenAsync();
                    SqlDataReader dr = await cmd.ExecuteReaderAsync();

                    if(await dr.ReadAsync())
                        return rowMapper(dr);

                    return default(T);
                }
            }
        }

        public Task<T> QueryForObjectAsync<T>(string sql, Func<SqlDataReader, T> rowMapper) where T : class {
            return QueryForObjectAsync(sql, new List<SqlParameter>(), rowMapper);
        }

        public async Task StoredProcedureAsync(string procedure, List<SqlParameter> parameters) {
            using(SqlConnection con = new SqlConnection(_options.Context)) {
                using(SqlCommand cmd = con.CreateCommand()) {
                    cmd.CommandText = procedure;
                    cmd.CommandType = CommandType.StoredProcedure;

                    parameters.ForEach(prm => cmd.Parameters.Add(prm));

                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
