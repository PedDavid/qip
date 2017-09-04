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

        public async Task<long> AddAsync(Board board, string userId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@name", SqlDbType.VarChar)
                    .Value = board.Name;

            parameters
                    .Add("@maxDistPoints", SqlDbType.TinyInt)
                    .Value = board.MaxDistPoints.Value;

            parameters
                    .Add("@basePermission", SqlDbType.TinyInt)
                    .Value = board.BasePermission;

            parameters
                    .Add("@userId", SqlDbType.VarChar)
                    .Value = userId??SqlString.Null;

            SqlParameter boardId = parameters.Add("@boardId", SqlDbType.BigInt);
            boardId.Direction = ParameterDirection.Output;

            await _queryTemplate.StoredProcedureAsync(INSERT_BOARD, parameters);

            return (long)boardId.Value;
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

        public Task<IEnumerable<Board>> GetAllAsync(long index, long size) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@take", SqlDbType.BigInt).Value = size;

            parameters.Add("@skip", SqlDbType.BigInt).Value = index * size;

            return _queryTemplate.QueryAsync(SELECT_ALL, parameters, GetBoard);
        }

        public Task<IEnumerable<Board>> GetAllAsync(long index, long size, string search) {
            if(search == null)
                return GetAllAsync(index, size);

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@take", SqlDbType.BigInt).Value = size;

            parameters.Add("@skip", SqlDbType.BigInt).Value = index * size;

            parameters.Add("@search", SqlDbType.NVarChar).Value = search;

            return _queryTemplate.QueryAsync(SELECT_SEARCH, parameters, GetBoard);
        }

        public Task RemoveAsync(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.StoredProcedureAsync(DELETE_BOARD, parameters);
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

            parameters
                .Add("@basePermission", SqlDbType.TinyInt)
                .Value = board.BasePermission;

            return _queryTemplate.CommandAsync(UPDATE_BOARD, parameters);
        }

        //SQL Commands
        private static readonly string BOARD_EXISTS = "SELECT CAST(count(id) as BIT) FROM dbo.Board WHERE id = @id";
        private static readonly string SELECT_ALL = "SELECT id, [name], maxDistPoints " +
                                                    "FROM dbo.Board " +
                                                    "ORDER BY id " +
                                                    "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
        private static readonly string SELECT_SEARCH = "SELECT id, [name], maxDistPoints " +
                                                       "FROM dbo.Board " +
                                                       "WHERE CONTAINS([name], @search)" +
                                                       "ORDER BY id " +
                                                       "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
        private static readonly string SELECT_BOARD = "SELECT id, [name], maxDistPoints FROM dbo.Board WHERE id = @id";
        private static readonly string UPDATE_BOARD = "UPDATE dbo.Board " +
                                                      "SET [name]= @name, " +
                                                          "maxDistPoints = @maxDistPoints, " +
                                                          "basePermission = @basePermission " +
                                                      "WHERE id = @id";

        //SQL Stored Procedures
        private static readonly string INSERT_BOARD = "[dbo].[InsertBoard]";
        private static readonly string DELETE_BOARD = "[dbo].[DeleteBoard]";


        //Extract Data From Data Reader
        private static Board GetBoard(SqlDataReader dr) {
            return new Board(dr.GetInt64(0)) {
                Name = dr.GetString(1),
                MaxDistPoints = dr.GetByte(2)
            };
        }
    }
}
