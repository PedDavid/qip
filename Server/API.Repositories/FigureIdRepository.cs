using API.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class FigureIdRepository : IFigureIdRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public FigureIdRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public Task<long> GetMaxIdAsync(long boardId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@boardId", SqlDbType.BigInt)
                    .Value = boardId;

            return _queryTemplate.QueryForScalarAsync<long>(SELECT_MAX_ID, parameters);
        }

        private static readonly string SELECT_MAX_ID = "SELECT isnull(max(id), -1) as maxId FROM dbo.Figure WHERE boardId = @boardId";
    }
}
