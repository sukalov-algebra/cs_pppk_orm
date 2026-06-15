using System;
using System.Reflection;
using orm.attributes;
using orm.sql;

namespace orm.ddl
{
    public static class AlterTableBuilder
    {
        public static string CreateTable(Type entityType)
        {
            var method = typeof(DdlBuilder<>)
                .MakeGenericType(entityType)
                .GetMethod(nameof(DdlBuilder<object>.CreateTable))!;
            return (string)method.Invoke(null, null)!;
        }

        public static string DropTable(string table) =>
            $"DROP TABLE {EntityColumns.Quote(table)};";

        public static string AddColumn(Type entityType, PropertyInfo property)
        {
            var column = EntityColumns.ColumnName(property);
            var sqlType = SqlTypeHelper.GetSqlType(property.PropertyType);

            var definition = $"{EntityColumns.Quote(column)} {sqlType}";

            var def = property.GetCustomAttribute<DefaultAttribute>();
            if (def != null) definition += $" DEFAULT {def.ToSql()}";

            return $"ALTER TABLE {EntityColumns.Quote(entityType.Name)} ADD COLUMN {definition};";
        }

        public static string AddColumn(string table, string column, string sqlType) =>
            $"ALTER TABLE {EntityColumns.Quote(table)} ADD COLUMN {EntityColumns.Quote(column)} {sqlType};";

        public static string DropColumn(string table, string column) =>
            $"ALTER TABLE {EntityColumns.Quote(table)} DROP COLUMN {EntityColumns.Quote(column)};";
    }
}
