namespace DynamicQuerying
{
    public enum QueryOperator
    {
        And,
        Or,
        End
    }

    internal class QueryOperators
    {
        public string Or
        {
            get { return " or "; }
        }

        public string And
        {
            get { return " and "; }
        }
    }
}