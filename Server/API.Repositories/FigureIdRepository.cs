using API.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class FigureIdRepository : IFigureIdRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public FigureIdRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public Task<long> GetMaxIdAsync() {
            return _queryTemplate.QueryForScalarAsync<long>(SELECT_MAX_ID);
        }

        private static readonly string SELECT_MAX_ID = "SELECT isnull(max(id), -1) as maxId FROM dbo.Figure";
    }
}
