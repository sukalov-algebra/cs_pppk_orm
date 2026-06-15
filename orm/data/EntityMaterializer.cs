using System;
using System.Collections.Generic;
using System.Globalization;
using Npgsql;
using orm.sql;

namespace orm.data
{
    public static class EntityMaterializer
    {
        public static void Populate(object entity, NpgsqlDataReader reader)
        {
            foreach (var p in EntityColumns.Mapped(entity.GetType()))
            {
                if (!p.CanWrite) continue;

                int ordinal;
                try { ordinal = reader.GetOrdinal(EntityColumns.ColumnName(p)); }
                catch (IndexOutOfRangeException) { continue; }

                var value = reader.GetValue(ordinal);
                if (value is DBNull) continue;

                p.SetValue(entity, Coerce(value, p.PropertyType));
            }
        }

        public static T Read<T>(NpgsqlDataReader reader) where T : new()
        {
            var entity = new T();
            Populate(entity!, reader);
            return entity;
        }

        public static List<T> ReadAll<T>(NpgsqlDataReader reader) where T : new()
        {
            var list = new List<T>();
            while (reader.Read())
                list.Add(Read<T>(reader));
            return list;
        }

        private static object Coerce(object value, Type target)
        {
            target = Nullable.GetUnderlyingType(target) ?? target;

            if (target.IsEnum) return Enum.ToObject(target, value);
            if (target.IsInstanceOfType(value)) return value;

            return Convert.ChangeType(value, target, CultureInfo.InvariantCulture);
        }
    }
}
