using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Repositories {
    public class SqlServerTemplate {
        private IConfiguration _configuration;

        public SqlServerTemplate(IConfiguration configuration) {
            _configuration = configuration;
        }

        public List<T> Query<T>(string sql, List<SqlParameter> parameters, Func<SqlDataReader, T> rowMapper) {
            List<T> result = new List<T>();
            using(SqlConnection con = new SqlConnection(_configuration.GetConnectionString("QIPContext"))) {
                using(SqlCommand cmd = con.CreateCommand()) {
                    cmd.CommandText = sql;

                    parameters.ForEach(prm => cmd.Parameters.Add(prm));

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while(dr.Read())
                        result.Add(rowMapper(dr));

                    return result;
                }
            }
        }

        public List<T> Query<T>(string sql, Func<SqlDataReader, T> rowMapper) {
            return Query(sql, new List<SqlParameter>(), rowMapper);
        }

        public void Query(string sql, List<SqlParameter> parameters) {
            using(SqlConnection con = new SqlConnection(_configuration.GetConnectionString("QIPContext"))) {
                using(SqlCommand cmd = con.CreateCommand()) {
                    cmd.CommandText = sql;

                    parameters.ForEach(prm => cmd.Parameters.Add(prm));

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Query(string sql) {
            Query(sql, new List<SqlParameter>());
        }

        public T QueryForScalar<T>(string sql, List<SqlParameter> parameters) {
            using(SqlConnection con = new SqlConnection(_configuration.GetConnectionString("QIPContext"))) {
                using(SqlCommand cmd = con.CreateCommand()) {
                    cmd.CommandText = sql;

                    parameters.ForEach(prm => cmd.Parameters.Add(prm));

                    con.Open();
                    return (T)cmd.ExecuteScalar();
                }
            }
        }

        public T QueryForScalar<T>(string sql) {
            return QueryForScalar<T>(sql, new List<SqlParameter>());
        }

        public T QueryForObject<T>(string sql, List<SqlParameter> parameters, Func<SqlDataReader, T> rowMapper) {
            using(SqlConnection con = new SqlConnection(_configuration.GetConnectionString("QIPContext"))) {
                using(SqlCommand cmd = con.CreateCommand()) {
                    cmd.CommandText = sql;

                    parameters.ForEach(prm => cmd.Parameters.Add(prm));

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if(dr.Read())
                        return rowMapper(dr);

                    return default(T);
                }
            }
        }

        public T QueryForObject<T>(string sql, Func<SqlDataReader, T> rowMapper) {
            return QueryForObject(sql, new List<SqlParameter>(), rowMapper);
        }

        public void StoredProcedure(string procedure, List<SqlParameter> parameters) {
            using(SqlConnection con = new SqlConnection(_configuration.GetConnectionString("QIPContext"))) {
                using(SqlCommand cmd = con.CreateCommand()) {
                    cmd.CommandText = procedure;
                    cmd.CommandType = CommandType.StoredProcedure;

                    parameters.ForEach(prm => cmd.Parameters.Add(prm));

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
