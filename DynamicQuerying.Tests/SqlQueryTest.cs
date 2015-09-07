using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicQuerying.Tests
{
    /// <summary>
    ///     Summary description for QueryTest
    /// </summary>
    [TestClass]
    public class SqlQueryTest
    {
        private const string PRODUCT_SCRIPT = "select * from [Production].[Product]";

        [TestMethod]
        public void test_ends_with()
        {
            Query query = new Query();
            query.Add(Criterion.Create<Products>(t => t.Name, "Chainring", CriteriaOperator.EndWith));

            string result = QueryTranslator.TranslateIntoSqlQuery(query, PRODUCT_SCRIPT);
            Assert.IsTrue(result == "exec sp_executesql N'select * from [Production].[Product] where (Name like ''%'' + @0)', N'@0 nvarchar(4000)', @0 = N'Chainring'");
        }

        [TestMethod]
        public void test_starts_with()
        {
            Query query = new Query();
            query.Add(Criterion.Create<Products>(t => t.Name, "Chainring", CriteriaOperator.StartWith));

            string result = QueryTranslator.TranslateIntoSqlQuery(query, PRODUCT_SCRIPT);
            Assert.IsTrue(result == "exec sp_executesql N'select * from [Production].[Product] where (Name like @0 + ''%'')', N'@0 nvarchar(4000)', @0 = N'Chainring'");
        }

        [TestMethod]
        public void TestSimpleContainQuery()
        {
            Query query = new Query();
            query.Add(Criterion.Create<Products>(t => t.ProductNumber, "CA-5965", CriteriaOperator.Equal));

            string result = QueryTranslator.TranslateIntoSqlQuery(query, PRODUCT_SCRIPT);
            Assert.IsTrue(result == "exec sp_executesql N'select * from [Production].[Product] where (ProductNumber = @0)', N'@0 nvarchar(4000)', @0 = N'CA-5965'");
        }

        [TestMethod]
        public void TestSimpleLikeQuery2()
        {
            Query query = new Query();
            query.Add(Criterion.Create<Products>(t => t.ProductId, 2, CriteriaOperator.Greater), QueryOperator.And);
            query.Add(Criterion.Create<Products>(t => t.ProductId, 4, CriteriaOperator.LesserThan));

            string result = QueryTranslator.TranslateIntoSqlQuery(query, PRODUCT_SCRIPT);
            Assert.IsTrue(result == "exec sp_executesql N'select * from [Production].[Product] where (ProductId > @0 and ProductId < @1)', N'@0 nvarchar(4000), @1 nvarchar(4000)', @0 = N'2', @1 = N'4'");
        }
    }
}