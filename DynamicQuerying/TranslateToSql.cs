using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;

namespace DynamicQuerying
{
    /// <summary>
    /// Class QueryTranslator.
    /// </summary>
    public static class QueryTranslator
    {
        /// <summary>
        /// Adds the paging.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="start">The start.</param>
        /// <param name="rowCount">The row count.</param>
        /// <returns>System.String.</returns>
        public static string AddPaging(this string query, int start, int rowCount)
        {
            return string.Format("Select * from ({0}) seq where seq.rownum between {1} and {2}", query, start, rowCount);
        }

        /// <summary>
        /// Adds the parameters.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="currentCount">The current count.</param>
        /// <returns>System.String.</returns>
        private static string AddParameters(Query query, int currentCount = 0)
        {
            StringBuilder sqlQuery = new StringBuilder();
            int count = query.FinalQuery.Count;
            int parametersCount = currentCount;

            for (int position = 0; position < count; position++)
            {
                sqlQuery.Append(CriteriaParametersType.NVarchar(parametersCount));

                if (position < (count - 1))
                    sqlQuery.Append(", ");

                parametersCount++;
            }
            return sqlQuery.ToString();
        }

        /// <summary>
        /// Addings the parameter values.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="currentCount">The current count.</param>
        /// <returns>System.String.</returns>
        private static string AddingParamValues(Query query, int currentCount = 0)
        {
            StringBuilder sqlQuery = new StringBuilder();
            int count = query.FinalQuery.Count;
            int parametersCount = currentCount;

            for (int position = 0; position < count; position++)
            {
                object value = query.FinalQuery[position].Item1.Value;
                sqlQuery.Append(string.Format("@{0} = N'{1}'", parametersCount, value));

                if (position < (count - 1))
                    sqlQuery.Append(", ");

                parametersCount++;
            }
            return sqlQuery.ToString();
        }

        /// <summary>
        /// Builds the ef query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="valuesList">The values list.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ApplicationException">No operator defined</exception>
        private static string BuildEfQuery(Query query, List<object> valuesList, int currentPosition = 0)
        {
            StringBuilder sqlQuery = new StringBuilder();
            List<object> values = valuesList;
            int parametersCount = currentPosition;

            if (query.FinalQuery == null || !query.FinalQuery.Any()) return sqlQuery.ToString();

            sqlQuery.Append("(");

            int totalCount = query.FinalQuery.Count();

            foreach (var c in query.FinalQuery)
            {
                totalCount--;
                switch (c.Item1.CriteriaOperator)
                {
                    case CriteriaOperator.Equal:
                        if (c.Item1.Value is Guid)
                        {
                            sqlQuery.Append(string.Format("{0}.Equals(@{1})", c.Item1.PropertyName, parametersCount));
                            values.Add(c.Item1.Value);
                        }
                        else if (c.Item1.Value is string)
                        {
                            sqlQuery.Append(string.Format("{0}.ToLower().Equals(@{1})", c.Item1.PropertyName.ToLower(), parametersCount));
                            values.Add(c.Item1.Value.ToString().ToLower());
                        }
                        else
                        {
                            sqlQuery.Append(string.Format("{0} = @{1}", c.Item1.PropertyName, parametersCount));
                            values.Add(c.Item1.Value);
                        }
                        break;
                    case CriteriaOperator.NotEqual:
                        sqlQuery.Append(string.Format("{0} != @{1}", c.Item1.PropertyName, parametersCount));
                        values.Add(c.Item1.Value);
                        break;
                    case CriteriaOperator.Like:
                        sqlQuery.Append(string.Format("{0}.ToLower().Contains(@{1})", c.Item1.PropertyName, parametersCount));
                        values.Add(c.Item1.Value.ToString().ToLower());
                        break;
                    case CriteriaOperator.StartWith:
                        sqlQuery.Append(string.Format("{0}.ToLower().StartsWith(@{1})", c.Item1.PropertyName, parametersCount));
                        values.Add(c.Item1.Value.ToString().ToLower());
                        break;
                    case CriteriaOperator.Contain:
                        sqlQuery.Append(string.Format("{0}.ToLower().Contains(@{1})", c.Item1.PropertyName, parametersCount));
                        values.Add(c.Item1.Value.ToString().ToLower());
                        break;
                    case CriteriaOperator.EndWith:
                        sqlQuery.Append(string.Format("{0}.ToLower().EndsWith(@{1})", c.Item1.PropertyName, parametersCount));
                        values.Add(c.Item1.Value.ToString().ToLower());
                        break;
                    case CriteriaOperator.GreaterThanOrEqual:
                        sqlQuery.Append(string.Format("{0} >= @{1}", c.Item1.PropertyName, parametersCount));
                        values.Add(c.Item1.Value);
                        break;
                    case CriteriaOperator.LesserThanOrEqual:
                        sqlQuery.Append(string.Format("{0} <= @{1}", c.Item1.PropertyName, parametersCount));
                        values.Add(c.Item1.Value);
                        break;
                    case CriteriaOperator.Greater:
                        sqlQuery.Append(string.Format("{0} > @{1}", c.Item1.PropertyName, parametersCount));
                        values.Add(c.Item1.Value);
                        break;
                    case CriteriaOperator.LesserThan:
                        sqlQuery.Append(string.Format("{0} < @{1}", c.Item1.PropertyName, parametersCount));
                        values.Add(c.Item1.Value);
                        break;
                    default:
                        throw new ApplicationException("No operator defined");
                }
                parametersCount++;
                if (totalCount > 0)
                {
                    QueryOperator queryOperator = c.Item2;
                    if (queryOperator != QueryOperator.End)
                        sqlQuery.Append(queryOperator == QueryOperator.And ? " and " : " or ");
                    else
                        break;
                }
            }


            sqlQuery.Append(")");

            return sqlQuery.ToString();
        }

        /// <summary>
        /// Builds the query from.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="currentCount">The current count.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ApplicationException">No operator defined</exception>
        private static string BuildQueryFrom(Query query, int currentCount = 0)
        {
            StringBuilder sqlQuery = new StringBuilder();

            if (query.FinalQuery != null)
            {
                int parametersCount = currentCount;
                int count = query.FinalQuery.Count();
                if (count > 0)
                    sqlQuery.Append("(");
                for (int position = 0; position < count; position++)
                {
                    switch (query.FinalQuery[position].Item1.CriteriaOperator)
                    {
                        case CriteriaOperator.Equal:
                            sqlQuery.Append(CriteriaParameter.Equal(query.FinalQuery[position].Item1.PropertyName, parametersCount));
                            break;
                        case CriteriaOperator.NotEqual:
                            sqlQuery.Append(CriteriaParameter.NotEqual(query.FinalQuery[position].Item1.PropertyName, parametersCount));
                            break;
                        case CriteriaOperator.Like:
                            sqlQuery.Append(CriteriaParameter.Like(query.FinalQuery[position].Item1.PropertyName, parametersCount));
                            break;
                        case CriteriaOperator.StartWith:
                            sqlQuery.Append(CriteriaParameter.StartWith(query.FinalQuery[position].Item1.PropertyName, parametersCount));
                            break;
                        case CriteriaOperator.Contain:
                            sqlQuery.Append(CriteriaParameter.Contain(query.FinalQuery[position].Item1.PropertyName, parametersCount));
                            break;
                        case CriteriaOperator.EndWith:
                            sqlQuery.Append(CriteriaParameter.EndWith(query.FinalQuery[position].Item1.PropertyName, parametersCount));
                            break;
                        case CriteriaOperator.Greater:
                            sqlQuery.Append(CriteriaParameter.GreaterThan(query.FinalQuery[position].Item1.PropertyName, parametersCount));
                            break;
                        case CriteriaOperator.LesserThan:
                            sqlQuery.Append(CriteriaParameter.LessThan(query.FinalQuery[position].Item1.PropertyName, parametersCount));
                            break;
                        case CriteriaOperator.GreaterThanOrEqual:
                            sqlQuery.Append(CriteriaParameter.GreaterThanOrEqual(query.FinalQuery[position].Item1.PropertyName, parametersCount));
                            break;
                        case CriteriaOperator.LesserThanOrEqual:
                            sqlQuery.Append(CriteriaParameter.LessThanOrEqual(query.FinalQuery[position].Item1.PropertyName, parametersCount));
                            break;
                        default:
                            throw new ApplicationException("No operator defined");
                    }
                    parametersCount++;

                    if (position < (count - 1))
                    {
                        QueryOperator queryOperator = query.FinalQuery[position].Item2;
                        if (queryOperator != QueryOperator.End)
                            sqlQuery.Append(queryOperator == QueryOperator.And ? " and " : " or ");
                        else
                        {
                            sqlQuery.Append(")");
                            break;
                        }
                    }
                    else
                        sqlQuery.Append(")");
                }
            }
            return sqlQuery.ToString();
        }

        /// <summary>
        /// Gets the member expressions.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns>Query.</returns>
        private static Query GetMemberExpressions(Expression body)
        {
            Query query = new Query();
            // A Queue preserves left to right reading order of expressions in the tree
            Queue<string> lastNodeType = new Queue<string>();
            Queue<Expression> candidates = new Queue<Expression>(new[] {body});
            while (candidates.Count > 0)
            {
                Expression expr = candidates.Dequeue();
                if (expr is MemberExpression)
                {
                    MemberExpression test = (MemberExpression) expr;
                    //yield return new Criterion("Name", currentValue, currentCriteria);
                }
                else if (expr is UnaryExpression)
                    candidates.Enqueue(((UnaryExpression) expr).Operand);
                else if (expr is BinaryExpression)
                {
                    BinaryExpression binary = expr as BinaryExpression;
                    string currentNodeType = binary.NodeType.ToString();
                    if (currentNodeType != "AndAlso" && currentNodeType != "OrElse")
                    {
                        CriteriaOperator currentCriteria = CriteriaOperator.Equal;
                        QueryOperator oOperator = QueryOperator.End;
                        if (lastNodeType.Any())
                        {
                            switch (lastNodeType.Dequeue())
                            {
                                case "AndAlso":
                                    oOperator = QueryOperator.And;
                                    break;
                                case "OrElse":
                                    oOperator = QueryOperator.Or;
                                    break;
                            }
                        }
                        else
                            oOperator = QueryOperator.End;
                        switch (currentNodeType)
                        {
                            case "Equal":
                                currentCriteria = CriteriaOperator.Equal;
                                break;
                            case "NotEqual":
                                currentCriteria = CriteriaOperator.NotEqual;
                                break;
                            case "GreaterThan":
                                currentCriteria = CriteriaOperator.Greater;
                                break;
                            case "GreaterThanOrEqual":
                                currentCriteria = CriteriaOperator.GreaterThanOrEqual;
                                break;
                            case "LessThan":
                                currentCriteria = CriteriaOperator.LesserThan;
                                break;
                            case "LessThanOrEqual":
                                currentCriteria = CriteriaOperator.LesserThanOrEqual;
                                break;
                            default:
                                currentCriteria = CriteriaOperator.Equal;
                                break;
                        }
                        object currentValue = ((dynamic) binary.Right).Value;
                        dynamic prop = ((dynamic) binary.Left).Member.Name;
                        query.Add(new Criterion(prop, currentValue, currentCriteria), oOperator);
                    }
                    else
                        lastNodeType.Enqueue(currentNodeType);
                    candidates.Enqueue(binary.Left);
                    candidates.Enqueue(binary.Right);
                }
                else if (expr is MethodCallExpression)
                {
                    MethodCallExpression method = expr as MethodCallExpression;
                    foreach (var argument in method.Arguments)
                        candidates.Enqueue(argument);
                }
                else if (expr is LambdaExpression)
                    candidates.Enqueue(((LambdaExpression) expr).Body);
            }
            return query;
        }

        /// <summary>
        /// To the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public static List<T> ToList<T>(this IQueryable queryable)
        {
            List<T> list = new List<T>();
            foreach (var item in queryable)
                list.Add((T) item);
            return list;
        }

        /// <summary>
        /// Translates the into ef query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        public static IQueryable<T> TranslateIntoEFQuery<T>(this IQueryable<T> source, Query query)
        {
            List<Object> values = new List<object>();
            string queryString = BuildEfQuery(query, values, 0);
            return !string.IsNullOrEmpty(queryString) ? source.Where(queryString, values.ToArray()) : source;
        }

        /// <summary>
        /// Translates the into ef query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="query">The query.</param>
        /// <param name="start">The start.</param>
        /// <param name="itemsCount">The items count.</param>
        /// <param name="primaryKeyName">Name of the primary key.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> TranslateIntoEFQuery<T>(this IQueryable<T> source, Query query, int start, int itemsCount, string primaryKeyName = null)
        {
            string primary = primaryKeyName ?? "Id";
            List<Object> values = new List<object>();
            string queryString = BuildEfQuery(query, values, 0);
            if (!string.IsNullOrEmpty(queryString))
            {
                return DynamicQueryable.Skip(source.Where(queryString, values.ToArray()).OrderBy(primary), start)
                    .Take(itemsCount)
                    .ToList<T>();
            }

            return DynamicQueryable.Skip(source.OrderBy(primary), start).Take(itemsCount).ToList<T>();
        }

        /// <summary>
        /// Translates the into SQL query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="initialQuery">The initial query.</param>
        /// <returns>System.String.</returns>
        public static string TranslateIntoSqlQuery(Query query, string initialQuery)
        {
            if (query.FinalQuery != null && query.FinalQuery.Any())
                return "exec sp_executesql N'" + initialQuery + " where " + BuildQueryFrom(query) + "', N'" + AddParameters(query) + "', " + AddingParamValues(query);

            return initialQuery;
        }

        /// <summary>
        /// Translates the into SQL query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="initialQuery">The initial query.</param>
        /// <returns>System.String.</returns>
        public static string TranslateIntoSqlQuery<T>(Expression<Func<T, bool>> whereClause, string initialQuery)
        {
            Query query = GetMemberExpressions(whereClause.Body);
            //var members = GetMemberExpressions(whereClause.Body);
            //var memberExpressions = members as MemberExpression[] ?? members.ToArray();
            //if (!memberExpressions.Any())
            //{
            //    throw new ArgumentException(
            //        "Not reducible to a Member Access",
            //        "predicate");
            //}

            //return memberExpressions.Select(m => m.Member.Name);
            return TranslateIntoSqlQuery(query, initialQuery);
        }
    }
}