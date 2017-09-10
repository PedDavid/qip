using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace API.Repositories.Extensions
{
    public static class SqlStringExtensions
    {
        public static string X(this SqlString sqlString) {//TODO CHANGE NAME
            return sqlString.IsNull ? null : sqlString.Value;
        }
    }
}
