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
    public class UserRepository : IUserRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public UserRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public long Add(User user) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters
                    .Add("@name", SqlDbType.VarChar)
                    .Value = user.Name;

                parameters
                    .Add("@username", SqlDbType.VarChar)
                    .Value = user.UserName;

                parameters
                    .Add("@pwdHash", SqlDbType.VarChar)
                    .Value = user.PwdHash;

                parameters
                    .Add("@pwdSalt", SqlDbType.VarChar)
                    .Value = user.PwdSalt;

                parameters
                    .Add("@favorites", SqlDbType.VarChar)
                    .Value = user.Favorites;

                parameters
                    .Add("@penColors", SqlDbType.VarChar)
                    .Value = user.PenColors;

                return _queryTemplate.QueryForScalar<long>(INSERT_USER, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public User Find(long id) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@id", SqlDbType.BigInt).Value = id;

                return _queryTemplate.QueryForObject(SELECT_USER, parameters, GetUser);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public IEnumerable<User> GetAll() {
            try {
                return _queryTemplate.Query(SELECT_ALL, GetUser);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public void Remove(long id) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@id", SqlDbType.BigInt).Value = id;

                _queryTemplate.Query(DELETE_USER, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public void Update(User user) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters
                        .Add("@id", SqlDbType.BigInt)
                        .Value = user.Id.Value;

                parameters
                     .Add("@name", SqlDbType.VarChar)
                    .Value = user.Name;

                parameters
                     .Add("@username", SqlDbType.VarChar)
                    .Value = user.UserName;

                parameters
                     .Add("@pwdHash", SqlDbType.VarChar)
                    .Value = user.PwdHash;

                parameters
                    .Add("@pwdSalt", SqlDbType.VarChar)
                    .Value = user.PwdSalt;

                parameters
                    .Add("@favorites", SqlDbType.VarChar)
                    .Value = user.Favorites;

                parameters
                    .Add("@penColors", SqlDbType.VarChar)
                    .Value = user.PenColors;

                _queryTemplate.Query(UPDATE_USER, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        //Note: This method can't alter favorites and penColors for null
        public void PartialUpdate(User user) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters
                        .Add("@id", SqlDbType.BigInt)
                        .Value = user.Id.Value;

                parameters
                     .Add("@name", SqlDbType.VarChar)
                    .Value = user.Name ?? SqlString.Null;

                parameters
                     .Add("@username", SqlDbType.VarChar)
                    .Value = user.UserName ?? SqlString.Null;

                parameters
                     .Add("@pwdHash", SqlDbType.VarChar)
                    .Value = user.PwdHash ?? SqlString.Null;

                parameters
                    .Add("@pwdSalt", SqlDbType.VarChar)
                    .Value = user.PwdSalt ?? SqlString.Null;


                parameters
                    .Add("@favorites", SqlDbType.VarChar)
                    .Value = user.Favorites ?? SqlString.Null;

                parameters
                    .Add("@penColors", SqlDbType.VarChar)
                    .Value = user.PenColors ?? SqlString.Null;

                _queryTemplate.Query(UPDATE_USER, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        //SQL Commands
        private static readonly string SELECT_ALL = "SELECT id, username, pwdHash, pwdSalt, name, favorites, penColors FROM dbo.[User]";
        private static readonly string SELECT_USER = "SELECT id, username, pwdHash, pwdSalt, name, favorites, penColors FROM dbo.[User] " +
                                                     "WHERE id = @id";
        private static readonly string INSERT_USER = "INSERT INTO dbo.[User] (name, pwdHash, pwdSalt, username, favorites, penColors) " +
                                                                    "VALUES (@name, @pwdHash, @pwdSalt, @username, @favorites, @penColors); " +
                                                     "SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";
        private static readonly string DELETE_USER = "DELETE FROM dbo.[User] WHERE id = @id";
        private static readonly string UPDATE_USER = "UPDATE dbo.[User] " +
                                                    "SET username = isnull(@username, username), " +
                                                        "pwdHash= isnull(@pwdHash, pwdHash), " +
                                                        "pwdSalt= isnull(@pwdSalt, pwdSalt), " +
                                                        "name= isnull(@name, name), " +
                                                        "favorites= @favorites, " +
                                                        "penColors= @penColors " +
                                                    "WHERE id = @id";

        //Extract Data From Data Reader
        private static User GetUser(SqlDataReader dr) {
            return new User(dr.GetInt64(0)) {
                UserName = dr.GetString(1),
                PwdHash = dr.GetString(2),
                PwdSalt = dr.GetString(3),
                Name = dr.GetString(4),
                Favorites = dr.GetString(5),
                PenColors = dr.GetString(6)
            };
        }
    }
}
