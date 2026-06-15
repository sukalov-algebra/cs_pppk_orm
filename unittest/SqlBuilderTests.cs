using orm.sql;
using unittest.Models;

namespace unittest
{
    public class SqlBuilderTests
    {
        [Fact]
        public void SelectAll_ReturnsSelectStarFromTable()
        {
            var sql = SqlBuilder<Product>.SelectAll();

            Assert.Equal("SELECT * FROM Product", sql);
        }

        [Fact]
        public void SelectCount_ReturnsCountFromTable()
        {
            var sql = SqlBuilder<Product>.SelectCount();

            Assert.Equal("SELECT COUNT(*) FROM Product", sql);
        }

        [Fact]
        public void Where_SimpleEquality_TranslatesToSqlComparison()
        {
            var sql = SqlBuilder<Product>.Where(x => x.Id == 1);

            Assert.Equal("SELECT * FROM Product WHERE \"Id\" = 1", sql);
        }

        [Fact]
        public void Where_AndCondition_TranslatesToAnd()
        {
            var sql = SqlBuilder<Product>.Where(x => x.Id == 1 && x.Name == "Widget");

            Assert.Equal("SELECT * FROM Product WHERE \"Id\" = 1 AND \"Name\" = 'Widget'", sql);
        }

        [Fact]
        public void Where_OrCondition_TranslatesToOr()
        {
            var sql = SqlBuilder<Product>.Where(x => x.Id == 1 || x.Id == 2);

            Assert.Equal("SELECT * FROM Product WHERE \"Id\" = 1 OR \"Id\" = 2", sql);
        }

        [Fact]
        public void Insert_BuildsColumnsAndValuesFromEntity()
        {
            var product = new Product { Id = 1, Name = "Widget", Price = 9.99m };

            var sql = SqlBuilder<Product>.Insert(product);

            Assert.Equal("INSERT INTO Product (\"Id\", \"Name\", \"Price\") VALUES (1, 'Widget', 9.99)", sql);
        }

        [Fact]
        public void Update_BuildsSetClauseAndWhereClause()
        {
            var product = new Product { Id = 1, Name = "Widget", Price = 9.99m };

            var sql = SqlBuilder<Product>.Update(product, x => x.Id == 1);

            Assert.Equal(
                "UPDATE Product SET \"Id\" = 1, \"Name\" = 'Widget', \"Price\" = 9.99 WHERE \"Id\" = 1",
                sql);
        }

        [Fact]
        public void Delete_BuildsDeleteWithWhereClause()
        {
            var sql = SqlBuilder<Product>.Delete(x => x.Id == 1);

            Assert.Equal("DELETE FROM Product WHERE \"Id\" = 1", sql);
        }
    }
}
