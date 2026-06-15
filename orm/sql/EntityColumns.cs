using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using orm.attributes;

namespace orm.sql
{
    public static class EntityColumns
    {
        public static IEnumerable<PropertyInfo> Mapped(Type type) =>
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && !IsNavigation(p));

        public static IEnumerable<PropertyInfo> Insertable(Type type) =>
            Mapped(type).Where(p => !IsIdentity(p));

        public static PropertyInfo? PrimaryKey(Type type) =>
            Mapped(type).FirstOrDefault(IsPrimaryKey);

        public static bool IsIdentity(PropertyInfo p) =>
            p.GetCustomAttribute<IdentityAttribute>() != null;

        public static bool IsPrimaryKey(PropertyInfo p) =>
            p.GetCustomAttribute<PrimaryKeyAttribute>() != null;

        public static bool IsNavigation(PropertyInfo p)
        {
            if (p.GetCustomAttribute<HasManyAttribute>() != null) return true;
            if (p.GetCustomAttribute<HasOneAttribute>() != null) return true;

            var t = p.PropertyType;
            if (t != typeof(string) && t != typeof(byte[]) && typeof(IEnumerable).IsAssignableFrom(t))
                return true;

            return false;
        }

        public static string ColumnName(PropertyInfo p)
        {
            var col = p.GetCustomAttribute<ColumnAttribute>();
            return string.IsNullOrWhiteSpace(col?.Name) ? p.Name : col!.Name!;
        }

        public static string Quote(string identifier) => $"\"{identifier}\"";

        public static string QuotedColumn(PropertyInfo p) => Quote(ColumnName(p));
    }
}
