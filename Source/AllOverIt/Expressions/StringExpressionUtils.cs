using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Expressions
{
    // TODO: tests
    public static class StringExpressionUtils
    {
        public static readonly MethodInfo CompareMethodInfo = typeof(string).GetMethod("Compare", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), typeof(string) }, null);
        public static readonly MethodInfo CompareStringComparisonMethodInfo = typeof(string).GetMethod("Compare", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), typeof(string), typeof(StringComparison) }, null);

        public static readonly MethodInfo ContainsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        public static readonly MethodInfo ContainsStringComparisonMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) });

        public static readonly MethodInfo StartsWithMethodInfo = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        public static readonly MethodInfo StartsWithStringComparisonMethodInfo = typeof(string).GetMethod("StartsWith", new[] { typeof(string), typeof(StringComparison) });

        public static readonly MethodInfo EndsWithMethodInfo = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        public static readonly MethodInfo EndsWithStringComparisonMethodInfo = typeof(string).GetMethod("EndsWith", new[] { typeof(string), typeof(StringComparison) });

        public static MethodCallExpression CreateCompareCallExpression(Expression instance, Expression value, StringComparison? stringComparison)
        {
            return stringComparison.HasValue
                ? CreateStaticComparisonCallExpression(CompareStringComparisonMethodInfo, instance, value, stringComparison.Value)
                : Expression.Call(CompareMethodInfo, instance, value);
        }

        public static MethodCallExpression CreateContainsCallExpression(Expression instance, Expression value, StringComparison? stringComparison)
        {
            return stringComparison.HasValue
                ? CreateInstanceComparisonCallExpression(ContainsStringComparisonMethodInfo, instance, value, stringComparison.Value)
                : Expression.Call(instance, ContainsMethodInfo, value);
        }

        public static MethodCallExpression CreateStartsWithCallExpression(Expression instance, Expression value, StringComparison? stringComparison)
        {
            return stringComparison.HasValue
                ? CreateInstanceComparisonCallExpression(StartsWithStringComparisonMethodInfo, instance, value, stringComparison.Value)
                : Expression.Call(instance, StartsWithMethodInfo, value);
        }

        public static MethodCallExpression CreateEndsWithCallExpression(Expression instance, Expression value, StringComparison? stringComparison)
        {
            return stringComparison.HasValue
                ? CreateInstanceComparisonCallExpression(EndsWithStringComparisonMethodInfo, instance, value, stringComparison.Value)
                : Expression.Call(instance, EndsWithMethodInfo, value);
        }

        private static MethodCallExpression CreateStaticComparisonCallExpression(MethodInfo methodInfo, Expression instance, Expression value, StringComparison stringComparison)
        {
            var comparison = Expression.Constant(stringComparison);
            return Expression.Call(methodInfo, instance, value, comparison);
        }

        private static MethodCallExpression CreateInstanceComparisonCallExpression(MethodInfo methodInfo, Expression instance, Expression value, StringComparison stringComparison)
        {
            var comparison = Expression.Constant(stringComparison);
            return Expression.Call(instance, methodInfo, value, comparison);
        }
    }
}
