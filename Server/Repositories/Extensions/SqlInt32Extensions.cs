using System.Data.SqlTypes;

namespace QIP.Repositories.Extensions {
    public static class SqlInt32Extensions {
        //Converte o SqlInt32 em int?, sendo o SqlInt32.Null equivalente a null
        public static int? ToNullableInt(this SqlInt32 sqlInt) {
            return sqlInt.IsNull ? null : (int?)sqlInt.Value;
        }
    }
}
