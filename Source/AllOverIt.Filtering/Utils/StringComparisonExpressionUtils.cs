using AllOverIt.Expressions;
using AllOverIt.Filtering.Extensions;
using AllOverIt.Filtering.Options;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Utils
{
    internal static class StringComparisonExpressionUtils
    {
        public static MethodCallExpression CreateCompareCallExpression(Expression value1, Expression value2, StringComparisonMode stringComparisonMode)
        {
            value1 = value1.ApplyStringComparisonMode(stringComparisonMode);
            value2 = value2.ApplyStringComparisonMode(stringComparisonMode);

            return StringExpressionUtils.CreateCompareCallExpression(value1, value2);
        }

        public static MethodCallExpression CreateContainsCallExpression(Expression instance, Expression value, StringComparisonMode stringComparisonMode)
        {
            instance = instance.ApplyStringComparisonMode(stringComparisonMode);
            value = value.ApplyStringComparisonMode(stringComparisonMode);

            return StringExpressionUtils.CreateContainsCallExpression(instance, value);
        }
    }
}