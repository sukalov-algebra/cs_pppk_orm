using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace orm.sql
{
    public static class SqlBuilder<T>
    {
        public static string Where(Expression<Func<T, bool>> predicate)
        {
            var visitor = new SqlExpressionVisitor();

            var whereClause = visitor.Translate(predicate.Body);

            return $"SELECT * FROM {typeof(T).Name} WHERE {whereClause}";
        }

        public static string Insert(T entity)
        {
            var properties = typeof(T).GetProperties();

            var columnNames =   string.Join(", ", properties.Select(p => SqlFormatHelper.GetColumnName(p)));
            var values =        string.Join(", ", properties.Select(p => SqlFormatHelper.GetSqlValue(p.GetValue(entity))));

            return $"INSERT INTO {typeof(T).Name} ({columnNames}) VALUES ({values})";
        }

        public static string Update(T entity, Expression<Func<T, bool>> predicate)
        {
            var properties = typeof(T).GetProperties();
            var visitor = new SqlExpressionVisitor();

            var setClause =     string.Join(", ", properties.Select(p => SqlFormatHelper.GetAssignment(p, entity)));
            var whereClause =   visitor.Translate(predicate.Body);

            return $"UPDATE {typeof(T).Name} SET {setClause} WHERE {whereClause}";
        }

        public static string Delete(Expression<Func<T, bool>> predicate)
        {
            var visitor = new SqlExpressionVisitor();

            var whereClause = visitor.Translate(predicate.Body);

            return $"DELETE FROM {typeof(T).Name} WHERE {whereClause}";
        }

        public static string SelectAll()
        {
            return $"SELECT * FROM {typeof(T).Name}";
        }

        public static string SelectCount()
        {
            return $"SELECT COUNT(*) FROM {typeof(T).Name}";
        }
    }
}
