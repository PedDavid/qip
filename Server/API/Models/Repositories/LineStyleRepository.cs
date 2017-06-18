using API.Models.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Repositories {
    public class LineStyleRepository : ILineStyleRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public LineStyleRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public long Add(LineStyle lineStyle) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters
                        .Add("@color", SqlDbType.VarChar)
                        .Value = lineStyle.Color;

                return _queryTemplate.QueryForScalar<long>(INSERT_LINE_STYLE, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public LineStyle Find(long id) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@id", SqlDbType.BigInt).Value = id;

                return _queryTemplate.QueryForObject(SELECT_LINE_STYLE, parameters, GetLineStyle);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public IEnumerable<LineStyle> GetAll() {
            try {
                return _queryTemplate.Query(SELECT_ALL, GetLineStyle);
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

                _queryTemplate.Query(DELETE_LINE_STYLE, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public void Update(LineStyle lineStyle) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters
                        .Add("@id", SqlDbType.BigInt)
                        .Value = lineStyle.Id.Value;

                parameters
                     .Add("@color", SqlDbType.VarChar)
                    .Value = lineStyle.Color;

                _queryTemplate.Query(UPDATE_LINE_STYLE, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public void PartialUpdate(LineStyle lineStyle) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters
                        .Add("@id", SqlDbType.BigInt)
                        .Value = lineStyle.Id.Value;

                parameters
                     .Add("@color", SqlDbType.VarChar)
                    .Value = lineStyle.Color ?? SqlString.Null;

                _queryTemplate.Query(UPDATE_LINE_STYLE, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
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
