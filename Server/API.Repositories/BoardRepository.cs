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

        public Task<long> Add(Board board) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@name", SqlDbType.VarChar)
                    .Value = board.Name;

            parameters
                    .Add("@maxDistPoints", SqlDbType.TinyInt)
                    .Value = board.MaxDistPoints.Value;

            return _queryTemplate.QueryForScalarAsync<long>(INSERT_BOARD, parameters);
        }

        public Task<Board> Find(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.QueryForObjectAsync(SELECT_BOARD, parameters, GetBoard);
        }

        public Task<IEnumerable<Board>> GetAll() {
            return _queryTemplate.QueryAsync(SELECT_ALL, GetBoard);
        }

        public Task Remove(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.QueryAsync(DELETE_BOARD, parameters);
        }

        public Task Update(Board board) {
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

            return _queryTemplate.QueryAsync(UPDATE_BOARD, parameters);
        }

        public Task PartialUpdate(Board board) {
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

            return _queryTemplate.QueryAsync(UPDATE_BOARD, parameters);
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
