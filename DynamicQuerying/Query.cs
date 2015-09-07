using System;
using System.Collections.Generic;

namespace DynamicQuerying
{
    public class Query
    {
        private readonly CompleteQuery _completeQuery = new CompleteQuery();

        internal CompleteQuery FinalQuery { get { return _completeQuery; } }

        public void Add(Criterion criterion)
        {
            FinalQuery.Add(Tuple.Create(criterion, QueryOperator.End));
        }
        public void Add(Criterion criterion, QueryOperator @operator)
        {
            FinalQuery.Add(Tuple.Create(criterion, @operator));
        }
    }

    internal class CompleteQuery : List<Tuple<Criterion, QueryOperator>>
    {
        
    }
}