using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories {
    public static class ParametersListExtensions {
        public static SqlParameter Add(this List<SqlParameter> parameters, string parameterName, SqlDbType dbType) {
            var parameter = new SqlParameter(parameterName, dbType);

            parameters.Add(parameter);

            return parameter;
        }
    }
}
