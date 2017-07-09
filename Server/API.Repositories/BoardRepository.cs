using API.Domain;
using API.Interfaces.IRepositories;
using API.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories {
    public class BoardRepository : IBoardRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public BoardRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public long Add(Board board) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters
                        .Add("@name", SqlDbType.VarChar)
                        .Value = board.Name;

                parameters
                        .Add("@maxDistPoints", SqlDbType.TinyInt)
                        .Value = board.MaxDistPoints.Value;

                return _queryTemplate.QueryForScalar<long>(INSERT_BOARD, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public Board Find(long id) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@id", SqlDbType.BigInt).Value = id;

                return _queryTemplate.QueryForObject(SELECT_BOARD, parameters, GetBoard);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public IEnumerable<Board> GetAll() {
            try {
                return _queryTemplate.Query(SELECT_ALL, GetBoard);
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

                _queryTemplate.Query(DELETE_BOARD, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public void Update(Board board) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters
                        .Add("@id", SqlDbType.BigInt)
                        .Value = board.Id.Value;

                parameters
                     .Add("@name", SqlDbType.VarChar)
                    .Value = board.Name;

                parameters
                    .Add("@maxDistPoints", SqlDbType.TinyInt)
                    .Value = board.MaxDistPoints.Value;

                _queryTemplate.Query(UPDATE_BOARD, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public void PartialUpdate(Board board) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters
                        .Add("@id", SqlDbType.BigInt)
                        .Value = board.Id.Value;

                parameters
                     .Add("@name", SqlDbType.VarChar)
                    .Value = board.Name ?? SqlString.Null;

                parameters
                    .Add("@maxDistPoints", SqlDbType.TinyInt)
                    .Value = board.MaxDistPoints ?? SqlByte.Null;

                _queryTemplate.Query(UPDATE_BOARD, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        //SQL Commands
        private static readonly string SELECT_ALL = "SELECT id, name, maxDistPoints FROM dbo.Board";
        private static readonly string SELECT_BOARD = "SELECT id, name, maxDistPoints FROM dbo.Board WHERE id = @id";
        private static readonly string INSERT_BOARD = "INSERT INTO dbo.Board (name, maxDistPoints) VALUES (@name, @maxDistPoints); " +
                                                     "SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";
        private static readonly string DELETE_BOARD = "DELETE FROM dbo.Board WHERE id = @id";
        private static readonly string UPDATE_BOARD = "UPDATE dbo.Board " +
                                                      "SET name= isnull(@name, name), " +
                                                          "maxDistPoints = isnull(@maxDistPoints, maxDistPoints)" +
                                                      "WHERE id = @id";

        //Extract Data From Data Reader
        private static Board GetBoard(SqlDataReader dr) {
            return new Board(dr.GetInt64(0)) {
                Name = dr.GetString(1),
                MaxDistPoints = dr.GetByte(2)
            };
        }
    }
}
