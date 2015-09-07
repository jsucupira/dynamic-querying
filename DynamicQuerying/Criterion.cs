using System;
using System.Linq.Expressions;

namespace DynamicQuerying
{
    public class Criterion
    {
        private readonly CriteriaOperator _criteriaOperator;

        public Criterion(string propertyName, object value, CriteriaOperator criteriaOperator)
        {
            PropertyName = propertyName;
            Value = value;
            _criteriaOperator = criteriaOperator;
        }

        public string PropertyName { get; private set; }
        public object Value { get; private set; }

        public CriteriaOperator CriteriaOperator
        {
            get { return _criteriaOperator; }
        }

        public static Criterion Create<T>(Expression<Func<T, object>> expression, object value, CriteriaOperator criteriaOperator)
        {
            string propertyName = PropertyNameHelper.ResolvePropertyName(expression);
            Criterion myCriterion = new Criterion(propertyName, value, criteriaOperator);
            return myCriterion;
        }
    }
}