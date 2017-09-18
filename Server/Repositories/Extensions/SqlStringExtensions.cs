using System.Data.SqlTypes;

namespace QIP.Repositories.Extensions {
    public static class SqlStringExtensions {
        //Converte o SqlString em String, sendo o SqlString.Null equivalente a null
        public static string ToNullableString(this SqlString sqlString) {
            return sqlString.IsNull ? null : sqlString.Value;
        }
    }
}
