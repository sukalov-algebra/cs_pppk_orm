using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using orm.attributes;
using orm.sql;

namespace orm.data
{
    public static class RelationLoader
    {
        public static void Load<T>(DbContext session, IEnumerable<T> parents)
        {
            var type = typeof(T);
            var navigations = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(EntityColumns.IsNavigation)
                .ToList();

            if (navigations.Count == 0) return;

            foreach (var parent in parents)
                if (parent != null)
                    foreach (var nav in navigations)
                        Load(session, type, parent, nav);
        }

        private static void Load(DbContext session, Type parentType, object parent, PropertyInfo nav)
        {
            var childType = ElementType(nav.PropertyType);
            if (childType != null)
                LoadMany(session, parentType, parent, nav, childType);
            else
                LoadOne(session, parentType, parent, nav);
        }

        private static void LoadMany(DbContext session, Type parentType, object parent, PropertyInfo nav, Type childType)
        {
            var parentKey = EntityColumns.PrimaryKey(parentType);
            if (parentKey == null) return;

            var keyValue = parentKey.GetValue(parent);
            if (keyValue == null) return;

            var foreignKey = ManyForeignKeyColumn(nav, parentType, childType);
            var children = session.SelectByColumnDynamic(childType, foreignKey, keyValue);
            nav.SetValue(parent, children);
        }

        private static void LoadOne(DbContext session, Type parentType, object parent, PropertyInfo nav)
        {
            var referencedType = nav.PropertyType;

            var foreignKeyProperty = OneForeignKeyProperty(nav, parentType, referencedType);
            if (foreignKeyProperty == null) return;

            var keyValue = foreignKeyProperty.GetValue(parent);
            if (keyValue == null) return;

            var referencedKey = EntityColumns.PrimaryKey(referencedType);
            if (referencedKey == null) return;

            var matches = (IList)session.SelectByColumnDynamic(
                referencedType, EntityColumns.ColumnName(referencedKey), keyValue);

            if (matches.Count > 0)
                nav.SetValue(parent, matches[0]);
        }

        private static string ManyForeignKeyColumn(PropertyInfo nav, Type parentType, Type childType)
        {
            var declared = nav.GetCustomAttribute<HasManyAttribute>()?.ForeignKey;
            if (!string.IsNullOrWhiteSpace(declared)) return declared!;

            var fk = EntityColumns.Mapped(childType)
                .FirstOrDefault(p => p.GetCustomAttribute<ForeignKeyAttribute>()?.References == parentType);
            if (fk != null) return EntityColumns.ColumnName(fk);

            return parentType.Name + "Id";
        }

        private static PropertyInfo? OneForeignKeyProperty(PropertyInfo nav, Type parentType, Type referencedType)
        {
            var declared = nav.GetCustomAttribute<HasOneAttribute>()?.ForeignKey;
            if (!string.IsNullOrWhiteSpace(declared))
                return parentType.GetProperty(declared!);

            var fk = EntityColumns.Mapped(parentType)
                .FirstOrDefault(p => p.GetCustomAttribute<ForeignKeyAttribute>()?.References == referencedType);
            if (fk != null) return fk;

            return parentType.GetProperty(referencedType.Name + "Id");
        }

        private static Type? ElementType(Type type)
        {
            if (type == typeof(string) || type == typeof(byte[])) return null;

            var enumerable = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return enumerable?.GetGenericArguments()[0];
        }
    }
}
