using QIP.Domain;
using QIP.Public.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace QIP.Repositories {
    public class LineStyleRepository : ILineStyleRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public LineStyleRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public Task<bool> ExistsAsync(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.QueryForScalarAsync<bool>(LINE_STYLE_EXISTS, parameters);
        }

        public async Task AddAsync(LineStyle lineStyle) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@color", SqlDbType.VarChar)
                    .Value = lineStyle.Color;

            lineStyle.Id = await _queryTemplate.QueryForScalarAsync<long>(INSERT_LINE_STYLE, parameters);
        }

        public Task<LineStyle> FindAsync(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.QueryForObjectAsync(SELECT_LINE_STYLE, parameters, GetLineStyle);
        }

        public Task<IEnumerable<LineStyle>> GetAllAsync(long index, long size) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@take", SqlDbType.BigInt).Value = size;

            parameters.Add("@skip", SqlDbType.BigInt).Value = index * size;

            return _queryTemplate.QueryAsync(SELECT_ALL, parameters, GetLineStyle);
        }

        public Task RemoveAsync(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.CommandAsync(DELETE_LINE_STYLE, parameters);
        }

        public Task UpdateAsync(LineStyle lineStyle) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@id", SqlDbType.BigInt)
                    .Value = lineStyle.Id.Value;

            parameters
                    .Add("@color", SqlDbType.VarChar)
                .Value = lineStyle.Color;

            return _queryTemplate.CommandAsync(UPDATE_LINE_STYLE, parameters);
        }

        //SQL Commands
        private static readonly string LINE_STYLE_EXISTS = "SELECT CAST(count(lineStyleId) as BIT) FROM dbo.LineStyle WHERE lineStyleId = @id";
        private static readonly string SELECT_ALL = "SELECT lineStyleId, color " +
                                                    "FROM dbo.LineStyle " +
                                                    "ORDER BY lineStyleId " +
                                                    "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
        private static readonly string SELECT_LINE_STYLE = "SELECT lineStyleId, color FROM dbo.LineStyle WHERE lineStyleId = @id";
        private static readonly string INSERT_LINE_STYLE = "INSERT INTO dbo.LineStyle (color) VALUES (@color); " +
                                                     "SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";
        private static readonly string DELETE_LINE_STYLE = "DELETE FROM dbo.LineStyle WHERE lineStyleId = @id";
        private static readonly string UPDATE_LINE_STYLE = "UPDATE dbo.LineStyle SET color = @color WHERE lineStyleId = @id";

        //Extract Data From Data Reader
        private static LineStyle GetLineStyle(SqlDataReader dr) {
            return new LineStyle() {
                Id = dr.GetInt64(0),
                Color = dr.GetString(1)
            };
        }
    }
}
