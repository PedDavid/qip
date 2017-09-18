using System.Threading.Tasks;
using QIP.Public.IRepositories;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace QIP.Repositories {
    public class FiguresRepository : IFiguresRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public FiguresRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public Task DeleteAsync(long boardId, long lastFigureToDelete = long.MaxValue) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            parameters.Add("@lastFigureToDelete", SqlDbType.BigInt).Value = lastFigureToDelete;

            return _queryTemplate.StoredProcedureAsync(DELETE_FIGURES, parameters);
        }


        //SQL Stored Procedures
        private static readonly string DELETE_FIGURES = "[dbo].[DeleteFigures]";
    }
}
