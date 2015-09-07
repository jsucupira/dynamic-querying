namespace DynamicQuerying
{
    internal class CriteriaOperators
    {
        public static string Equal(int position, object value)
        {
            return string.Format("@{0} = N'{1}'", position, value);
        }

        public static string NotEqual(int position, object value)
        {
            return string.Format("@{0} <> N'{1}'", position, value);
        }

        public static string Like(int position, object value)
        {
            return string.Format("@{0} = N'{1}'", position, value);
        }

        public static string StartWith(int position, object value)
        {
            return string.Format("@{0} = N'{1}%'", position, value);
        }

        public static string Contain(int position, object value)
        {
            return string.Format("@{0} = N'%{1}%'", position, value);
        }

        public static string EndWith(int position, object value)
        {
            return string.Format("@{0} = N'%{1}'", position, value);
        }
    }

    internal class CriteriaParameter
    {
        public static string Equal(string propertyName, int position)
        {
            return string.Format("{0} = @{1}", propertyName, position);
        }

        public static string NotEqual(string propertyName, int position)
        {
            return string.Format("{0} <> @{1}", propertyName, position);
        }

        public static string StartWith(string propertyName, int position)
        {
            return string.Format("{0} like @{1} + ''%''", propertyName, position);
        }
        public static string Contain(string propertyName, int position)
        {
            return string.Format("{0} like ''%'' + @{1} + ''%''", propertyName, position);
        }
        public static string EndWith(string propertyName, int position)
        {
            return string.Format("{0} like ''%'' + @{1}", propertyName, position);
        }

        public static string Like(string propertyName, int position)
        {
            return string.Format("{0} like @{1}", propertyName, position);
        }


        public static string GreaterThan(string propertyName, int position)
        {
            return string.Format("{0} > @{1}", propertyName, position);
        }

        public static string LessThan(string propertyName, int position)
        {
            return string.Format("{0} < @{1}", propertyName, position);
        }

        public static string LessThanOrEqual(string propertyName, int position)
        {
            return string.Format("{0} <= @{1}", propertyName, position);
        }

        public static string GreaterThanOrEqual(string propertyName, int position)
        {
            return string.Format("{0} >= @{1}", propertyName, position);
        }
    }

    public class CriteriaParametersType
    {
        public static string Integer(int position)
        {
            return string.Format("@{0} int", position);
        }

        public static string NVarchar(int position)
        {
            return string.Format("@{0} nvarchar(4000)", position);
        }
    }
}