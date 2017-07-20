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

        public Task<long> AddAsync(Board board) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@name", SqlDbType.VarChar)
                    .Value = board.Name;

            parameters
                    .Add("@maxDistPoints", SqlDbType.TinyInt)
                    .Value = board.MaxDistPoints.Value;

            return _queryTemplate.QueryForScalarAsync<long>(INSERT_BOARD, parameters);
        }

        public Task<Board> FindAsync(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.QueryForObjectAsync(SELECT_BOARD, parameters, GetBoard);
        }

        public Task<bool> ExistsAsync(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.QueryForScalarAsync<bool>(BOARD_EXISTS, parameters);
        }

        public Task<IEnumerable<Board>> GetAllAsync() {
            return _queryTemplate.QueryAsync(SELECT_ALL, GetBoard);
        }

        public Task RemoveAsync(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.CommandAsync(DELETE_BOARD, parameters);
        }

        public Task UpdateAsync(Board board) {
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

            return _queryTemplate.CommandAsync(UPDATE_BOARD, parameters);
        }

        public Task PartialUpdateAsync(Board board) {
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

            return _queryTemplate.CommandAsync(UPDATE_BOARD, parameters);
        }

        //SQL Commands
        private static readonly string BOARD_EXISTS = "SELECT CAST(count(id) as BIT) FROM dbo.Board WHERE id = @id";
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
