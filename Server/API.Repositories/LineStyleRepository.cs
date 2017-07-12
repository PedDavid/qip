using API.Domain;
using API.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories {
    public class LineStyleRepository : ILineStyleRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public LineStyleRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public Task<long> Add(LineStyle lineStyle) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@color", SqlDbType.VarChar)
                    .Value = lineStyle.Color;

            return _queryTemplate.QueryForScalarAsync<long>(INSERT_LINE_STYLE, parameters);
        }

        public Task<LineStyle> Find(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.QueryForObjectAsync(SELECT_LINE_STYLE, parameters, GetLineStyle);
        }

        public Task<IEnumerable<LineStyle>> GetAll() {
            return _queryTemplate.QueryAsync(SELECT_ALL, GetLineStyle);
        }

        public Task Remove(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.QueryAsync(DELETE_LINE_STYLE, parameters);
        }

        public Task Update(LineStyle lineStyle) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@id", SqlDbType.BigInt)
                    .Value = lineStyle.Id.Value;

            parameters
                    .Add("@color", SqlDbType.VarChar)
                .Value = lineStyle.Color;

            return _queryTemplate.QueryAsync(UPDATE_LINE_STYLE, parameters);
        }

        public Task PartialUpdate(LineStyle lineStyle) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@id", SqlDbType.BigInt)
                    .Value = lineStyle.Id.Value;

            parameters
                    .Add("@color", SqlDbType.VarChar)
                .Value = lineStyle.Color ?? SqlString.Null;

            return _queryTemplate.QueryAsync(UPDATE_LINE_STYLE, parameters);
        }

        //SQL Commands
        private static readonly string SELECT_ALL = "SELECT lineStyleId, color FROM dbo.LineStyle";
        private static readonly string SELECT_LINE_STYLE = "SELECT lineStyleId, color FROM dbo.LineStyle WHERE lineStyleId = @id";
        private static readonly string INSERT_LINE_STYLE = "INSERT INTO dbo.LineStyle (color) VALUES (@color); " +
                                                     "SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";
        private static readonly string DELETE_LINE_STYLE = "DELETE FROM dbo.LineStyle WHERE lineStyleId = @id";
        private static readonly string UPDATE_LINE_STYLE = "UPDATE dbo.LineStyle SET color = isnull(@color, color) WHERE lineStyleId = @id";

        //Extract Data From Data Reader
        private static LineStyle GetLineStyle(SqlDataReader dr) {
            return new LineStyle(dr.GetInt64(0)) {
                Color = dr.GetString(1)
            };
        }
    }
}
