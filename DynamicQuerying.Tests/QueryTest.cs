using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicQuerying.Tests
{
    /// <summary>
    ///     Summary description for QueryTest
    /// </summary>
    [TestClass]
    public class QueryTest
    {
        private IQueryable<Products> _products;

        [TestMethod]
        public void test_expression_toQuery()
        {
            string result = QueryTranslator.TranslateIntoSqlQuery<Products>(t => t.Name != "HelloWorld" || t.ProductId >= 10 && t.ProductId > 2, "select * from company");
        }

        [TestMethod]
        public void TestFirstOrDefault()
        {
            Products expected = _products.FirstOrDefault("Name = @0", "Floor");

            Assert.IsNotNull(expected);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            List<Products> temp = new List<Products>();
            temp.Add(new Products
            {
                Name = "House",
                ProductId = 1,
                ProductNumber = "123"
            });
            temp.Add(new Products
            {
                Name = "bike",
                ProductId = 2,
                ProductNumber = "1234"
            });
            temp.Add(new Products
            {
                Name = "Table",
                ProductId = 3,
                ProductNumber = "1a213"
            });
            temp.Add(new Products
            {
                Name = "Chair",
                ProductId = 4,
                ProductNumber = "poaf90123"
            });
            temp.Add(new Products
            {
                Name = "Cable",
                ProductId = 5,
                ProductNumber = "poaf908f"
            });
            temp.Add(new Products
            {
                Name = "Computer",
                ProductId = 6,
                ProductNumber = "123-adsca354"
            });
            temp.Add(new Products
            {
                Name = "Floor",
                ProductId = 7,
                ProductNumber = "123"
            });
            temp.Add(new Products
            {
                Name = "Sea",
                ProductId = 8,
                ProductNumber = "546234ew"
            });
            temp.Add(new Products
            {
                Name = "Cup",
                ProductId = 9,
                ProductNumber = "k-wer"
            });
            temp.Add(new Products
            {
                Name = "Apartment",
                ProductId = 10,
                ProductNumber = "123"
            });
            temp.Add(new Products
            {
                Name = "Apartment",
                ProductId = 11,
                ProductNumber = "124"
            });
            temp.Add(new Products
            {
                Name = "Apartment",
                ProductId = 12,
                ProductNumber = "1das"
            });
            _products = temp.AsQueryable();
        }

        [TestMethod]
        public void TestSimpleContainQuery()
        {
            Query query = new Query();
            Criterion criterion = new Criterion("ProductNumber", "ads", CriteriaOperator.Contain);
            query.Add(criterion);
            const int expected = 1;

            Assert.AreEqual(expected, _products.TranslateIntoEFQuery(query).Count());
        }

        [TestMethod]
        public void TestSimpleContainQuery2()
        {
            Query query = new Query();
            query.Add(Criterion.Create<Products>(t => t.ProductNumber, "ads", CriteriaOperator.Contain));
            const int expected = 1;
            Assert.AreEqual(expected, _products.TranslateIntoEFQuery(query).Count());
        }

        [TestMethod]
        public void TestSimpleEqualQuery()
        {
            Query query = new Query();
            Criterion criterion = new Criterion("ProductNumber", "123", CriteriaOperator.Equal);
            query.Add(criterion);
            const int expected = 3;

            Assert.AreEqual(expected, _products.TranslateIntoEFQuery(query).Count());
        }

        [TestMethod]
        public void TestSimpleEqualQuery2()
        {
            Query query = new Query();
            query.Add(Criterion.Create<Products>(t => t.ProductNumber, "123", CriteriaOperator.Equal));
            const int expected = 3;

            Assert.AreEqual(expected, _products.TranslateIntoEFQuery(query).Count());
        }

        [TestMethod]
        public void TestSimpleLikeQuery2()
        {
            Query query = new Query();
            query.Add(Criterion.Create<Products>(t => t.ProductId, 2, CriteriaOperator.Greater), QueryOperator.And);
            query.Add(Criterion.Create<Products>(t => t.ProductId, 4, CriteriaOperator.LesserThan));
            const int expected = 1;

            Assert.AreEqual(expected, _products.TranslateIntoEFQuery(query).Count());
        }

        [TestMethod]
        public void TestSimpleLikeQuery3()
        {
            Query query = new Query();
            query.Add(Criterion.Create<Products>(t => t.Name, "House", CriteriaOperator.Equal), QueryOperator.Or);
            query.Add(Criterion.Create<Products>(t => t.Name, "Bike", CriteriaOperator.Equal), QueryOperator.Or);
            query.Add(Criterion.Create<Products>(t => t.Name, "Apartment", CriteriaOperator.Equal));
            const int expected = 5;
            int result = _products.TranslateIntoEFQuery(query).Count();

            Assert.AreEqual(expected, result);
        }
    }
}