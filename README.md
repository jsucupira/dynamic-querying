# dynamic-querying
This library allows for dynamically created sql query to be executed.

Here is some examples:
=======
**Example 1.**

    Query query = new Query();
    query.Add(Criterion.Create<Products>(t => t.ProductNumber, "ads", CriteriaOperator.Contain));
    const int expected = 1;
    Assert.AreEqual(expected, _products.TranslateIntoEFQuery(query).Count());

**Example 2.**

    Query query = new Query();
    query.Add(Criterion.Create<Products>(t => t.Name, "House", CriteriaOperator.Equal), QueryOperator.Or);
    query.Add(Criterion.Create<Products>(t => t.Name, "Bike", CriteriaOperator.Equal), QueryOperator.Or);
    query.Add(Criterion.Create<Products>(t => t.Name, "Apartment", CriteriaOperator.Equal));
    const int expected = 5;
    int result = _products.TranslateIntoEFQuery(query).Count();

*More examples can be found in the unit tests.*

