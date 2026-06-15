using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Npgsql;
using orm.ddl;
using orm.sql;

namespace orm.data
{
    public sealed class DbContext : IDisposable
    {
        private readonly NpgsqlConnection _connection;
        private NpgsqlTransaction? _transaction;

        public DbContext(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
        }

        public void BeginTransaction() => _transaction = _connection.BeginTransaction();

        public void Commit()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            _transaction = null;
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }

        public void CreateTable<T>() => Execute(DdlBuilder<T>.CreateTable());

        public T Insert<T>(T entity) where T : new()
        {
            using var reader = ExecuteReader(SqlBuilder<T>.Insert(entity));
            if (reader.Read())
                EntityMaterializer.Populate(entity!, reader);
            return entity;
        }

        public int Update<T>(T entity, Expression<Func<T, bool>> predicate) =>
            Execute(SqlBuilder<T>.Update(entity, predicate));

        public void UpdateByPrimaryKey<T>(T entity) =>
            Execute(SqlBuilder<T>.UpdateByPrimaryKey(entity));

        internal void UpdateByPrimaryKeyDynamic(object entity)
        {
            typeof(DbContext)
                .GetMethod(nameof(UpdateByPrimaryKey))!
                .MakeGenericMethod(entity.GetType())
                .Invoke(this, new[] { entity });
        }

        public int Delete<T>(Expression<Func<T, bool>> predicate) =>
            Execute(SqlBuilder<T>.Delete(predicate));

        public List<T> Select<T>(
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = false,
            bool loadRelated = false) where T : new()
        {
            List<T> result;
            using (var reader = ExecuteReader(SqlBuilder<T>.Select(predicate, orderBy, descending)))
                result = EntityMaterializer.ReadAll<T>(reader);

            if (loadRelated)
                RelationLoader.Load(this, result);

            return result;
        }

        public List<T> All<T>(
            Expression<Func<T, object>>? orderBy = null,
            bool descending = false,
            bool loadRelated = false) where T : new() =>
            Select(null, orderBy, descending, loadRelated);

        public long Count<T>() => Convert.ToInt64(ExecuteScalar(SqlBuilder<T>.SelectCount()));

        public List<T> SelectByColumn<T>(string column, object value) where T : new()
        {
            using var reader = ExecuteReader(SqlBuilder<T>.SelectByColumn(column, value));
            return EntityMaterializer.ReadAll<T>(reader);
        }

        internal object SelectByColumnDynamic(Type entityType, string column, object value)
        {
            var method = typeof(DbContext)
                .GetMethod(nameof(SelectByColumn))!
                .MakeGenericMethod(entityType);
            return method.Invoke(this, new[] { column, value })!;
        }

        public int Execute(string sql)
        {
            using var command = Command(sql);
            return command.ExecuteNonQuery();
        }

        internal NpgsqlDataReader ExecuteReader(string sql) => Command(sql).ExecuteReader();

        private object? ExecuteScalar(string sql)
        {
            using var command = Command(sql);
            return command.ExecuteScalar();
        }

        private NpgsqlCommand Command(string sql) => new(sql, _connection, _transaction);

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection.Dispose();
        }
    }
}
