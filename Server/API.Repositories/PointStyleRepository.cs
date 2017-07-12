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
    public class PointStyleRepository : IPointStyleRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public PointStyleRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public Task<long> AddAsync(PointStyle pointStyle) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@width", SqlDbType.Int)
                    .Value = pointStyle.Width;

            return _queryTemplate.QueryForScalarAsync<long>(INSERT_POINT_STYLE, parameters);
        }

        public Task<PointStyle> FindAsync(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.QueryForObjectAsync(SELECT_POINT_STYLE, parameters, GetPointStyle);
        }

        public Task<IEnumerable<PointStyle>> GetAllAsync() {
            return _queryTemplate.QueryAsync(SELECT_ALL, GetPointStyle);
        }

        public Task RemoveAsync(long id) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            return _queryTemplate.QueryAsync(DELETE_POINT_STYLE, parameters);
        }

        public Task UpdateAsync(PointStyle pointStyle) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@id", SqlDbType.BigInt)
                    .Value = pointStyle.Id.Value;

            parameters
                    .Add("@width", SqlDbType.Int)
                .Value = pointStyle.Width;

            return _queryTemplate.QueryAsync(UPDATE_POINT_STYLE, parameters);
        }

        public Task PartialUpdateAsync(PointStyle pointStyle) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@id", SqlDbType.BigInt)
                    .Value = pointStyle.Id.Value;

            parameters
                    .Add("@width", SqlDbType.Int)
                .Value = pointStyle.Width ?? SqlInt32.Null;

            return _queryTemplate.QueryAsync(UPDATE_POINT_STYLE, parameters);
        }

        //SQL Commands
        private static readonly string SELECT_ALL = "SELECT pointStyleId, width FROM dbo.PointStyle";
        private static readonly string SELECT_POINT_STYLE = "SELECT pointStyleId, width FROM dbo.PointStyle WHERE pointStyleId = @id";
        private static readonly string INSERT_POINT_STYLE = "INSERT INTO dbo.PointStyle (width) VALUES (@width); " +
                                                     "SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";
        private static readonly string DELETE_POINT_STYLE = "DELETE FROM dbo.PointStyle WHERE pointStyleId = @id";
        private static readonly string UPDATE_POINT_STYLE = "UPDATE dbo.PointStyle SET width = isnull(@width, width) WHERE pointStyleId = @id";

        //Extract Data From Data Reader
        private static PointStyle GetPointStyle(SqlDataReader dr) {
            return new PointStyle(dr.GetInt64(0)) {
                Width = dr.GetInt32(1)
            };
        }
    }
}
