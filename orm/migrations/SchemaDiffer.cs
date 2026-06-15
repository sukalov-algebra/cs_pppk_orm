using System;
using System.Collections.Generic;
using System.Linq;
using orm.ddl;
using orm.sql;

namespace orm.migrations
{
    public static class SchemaDiffer
    {
        public static List<SchemaChange> Diff(
            IEnumerable<Type> entities,
            Dictionary<string, Dictionary<string, string>> current)
        {
            var changes = new List<SchemaChange>();

            foreach (var entity in entities)
            {
                var table = entity.Name;
                var modelColumns = EntityColumns.Mapped(entity).ToList();

                if (!current.TryGetValue(table, out var existingColumns))
                {
                    changes.Add(new SchemaChange(
                        SchemaChangeKind.CreateTable,
                        $"CreateTable_{table}",
                        AlterTableBuilder.CreateTable(entity),
                        AlterTableBuilder.DropTable(table)));
                    continue;
                }

                foreach (var property in modelColumns)
                {
                    var column = EntityColumns.ColumnName(property);
                    if (existingColumns.ContainsKey(column)) continue;

                    changes.Add(new SchemaChange(
                        SchemaChangeKind.AddColumn,
                        $"AddColumn_{table}_{column}",
                        AlterTableBuilder.AddColumn(entity, property),
                        AlterTableBuilder.DropColumn(table, column)));
                }

                var modelColumnNames = modelColumns.Select(EntityColumns.ColumnName).ToHashSet(StringComparer.Ordinal);

                foreach (var existing in existingColumns)
                {
                    if (modelColumnNames.Contains(existing.Key)) continue;

                    changes.Add(new SchemaChange(
                        SchemaChangeKind.DropColumn,
                        $"DropColumn_{table}_{existing.Key}",
                        AlterTableBuilder.DropColumn(table, existing.Key),
                        AlterTableBuilder.AddColumn(table, existing.Key, existing.Value)));
                }
            }

            return changes;
        }
    }
}
