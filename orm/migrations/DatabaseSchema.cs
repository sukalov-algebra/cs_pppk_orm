using System;
using System.Collections.Generic;
using orm.data;

namespace orm.migrations
{
    public static class DatabaseSchema
    {
        public static Dictionary<string, Dictionary<string, string>> Read(DbContext session)
        {
            const string sql =
                "SELECT table_name, column_name, data_type " +
                "FROM information_schema.columns " +
                "WHERE table_schema = 'public'";

            var schema = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);

            using var reader = session.ExecuteReader(sql);
            while (reader.Read())
            {
                var table = reader.GetString(0);
                var column = reader.GetString(1);
                var dataType = reader.GetString(2);

                if (!schema.TryGetValue(table, out var columns))
                    schema[table] = columns = new Dictionary<string, string>(StringComparer.Ordinal);

                columns[column] = dataType;
            }

            return schema;
        }
    }
}
