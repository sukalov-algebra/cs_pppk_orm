using System;
using System.Collections.Generic;
using System.Text;

namespace orm.sql
{
    internal static class SqlTypeHelper
    {
        internal static readonly Dictionary<Type, string> PostgresTypeMap = new()
        {
            { typeof(string), "TEXT" },
            { typeof(int), "INTEGER" },
            { typeof(long), "BIGINT" },
            { typeof(short), "SMALLINT" },
            { typeof(bool), "BOOLEAN" },
            { typeof(DateTime), "TIMESTAMP" },
            { typeof(DateTimeOffset), "TIMESTAMPTZ" },
            { typeof(Guid), "UUID" },
            { typeof(decimal), "NUMERIC" },
            { typeof(double), "DOUBLE PRECISION" },
            { typeof(float), "REAL" }
        };

        internal static string GetSqlType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            return PostgresTypeMap.TryGetValue(type, out var sqlType)
                ? sqlType
                : "TEXT";
        }
    }
}
