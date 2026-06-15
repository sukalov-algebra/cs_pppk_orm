using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace orm.sql
{
    public static class SqlBuilder<T>
    {
        public static string Select(
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = false)
        {
            var sb = new StringBuilder($"SELECT * FROM {Table()}");

            if (predicate != null)
                sb.Append(" WHERE ").Append(new SqlExpressionVisitor().Translate(predicate.Body));

            if (orderBy != null)
                sb.Append(" ORDER BY ")
                  .Append(new SqlExpressionVisitor().Translate(orderBy.Body))
                  .Append(descending ? " DESC" : " ASC");

            return sb.ToString();
        }

        public static string Where(Expression<Func<T, bool>> predicate) => Select(predicate);

        public static string SelectAll() => Select();

        public static string SelectByColumn(string column, object? value) =>
            $"SELECT * FROM {Table()} WHERE {EntityColumns.Quote(column)} = {SqlFormatHelper.GetSqlValue(value)}";

        public static string SelectCount() => $"SELECT COUNT(*) FROM {Table()}";

        public static string Insert(T entity)
        {
            var properties = EntityColumns.Insertable(typeof(T)).ToList();

            var columnNames = string.Join(", ", properties.Select(EntityColumns.QuotedColumn));
            var values =      string.Join(", ", properties.Select(p => SqlFormatHelper.GetSqlValue(p.GetValue(entity))));

            return $"INSERT INTO {Table()} ({columnNames}) VALUES ({values}) RETURNING *";
        }

        public static string Update(T entity, Expression<Func<T, bool>> predicate)
        {
            var setClause = SetClause(entity);
            var whereClause = new SqlExpressionVisitor().Translate(predicate.Body);

            return $"UPDATE {Table()} SET {setClause} WHERE {whereClause}";
        }

        public static string UpdateByPrimaryKey(T entity)
        {
            var pk = EntityColumns.PrimaryKey(typeof(T))
                ?? throw new InvalidOperationException($"Type {typeof(T).Name} has no [PrimaryKey] property.");

            var setClause = SetClause(entity);
            var keyValue = SqlFormatHelper.GetSqlValue(pk.GetValue(entity));

            return $"UPDATE {Table()} SET {setClause} WHERE {EntityColumns.QuotedColumn(pk)} = {keyValue}";
        }

        public static string Delete(Expression<Func<T, bool>> predicate)
        {
            var whereClause = new SqlExpressionVisitor().Translate(predicate.Body);

            return $"DELETE FROM {Table()} WHERE {whereClause}";
        }

        private static string SetClause(T entity) =>
            string.Join(", ", EntityColumns.Insertable(typeof(T))
                .Select(p => $"{EntityColumns.QuotedColumn(p)} = {SqlFormatHelper.GetSqlValue(p.GetValue(entity))}"));

        private static string Table() => EntityColumns.Quote(typeof(T).Name);
    }
}
