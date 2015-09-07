using System;
using System.Linq.Expressions;

namespace DynamicQuerying
{
    public static class PropertyNameHelper
    {
        public static string ResolvePropertyName<T>(Expression<Func<T, object>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                UnaryExpression unaryExpress = expression.Body as UnaryExpression;
                if (unaryExpress != null) memberExpression = unaryExpress.Operand as MemberExpression;
            }
            if (memberExpression != null)
                return memberExpression.ToString().Substring(memberExpression.ToString().IndexOf(".", System.StringComparison.Ordinal) + 1);

            return null;
        }
    }
}