using AllOverIt.Assertion;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Expressions
{
    /// <summary>A utility class that provides expression helpers related string operations.</summary>
    public static class StringExpressionUtils
    {
        private static readonly MethodInfo CompareMethodInfo = typeof(string).GetMethod("Compare", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), typeof(string) }, null);
        private static readonly MethodInfo CompareStringComparisonMethodInfo = typeof(string).GetMethod("Compare", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), typeof(string), typeof(StringComparison) }, null);

        private static readonly MethodInfo ContainsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static readonly MethodInfo ContainsStringComparisonMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) });

        private static readonly MethodInfo StartsWithMethodInfo = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static readonly MethodInfo StartsWithStringComparisonMethodInfo = typeof(string).GetMethod("StartsWith", new[] { typeof(string), typeof(StringComparison) });

        private static readonly MethodInfo EndsWithMethodInfo = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        private static readonly MethodInfo EndsWithStringComparisonMethodInfo = typeof(string).GetMethod("EndsWith", new[] { typeof(string), typeof(StringComparison) });

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the static method <see cref="string.Compare(string?, string?)" /> or
        /// <see cref="string.Compare(string?, string?, StringComparison)"/> method based on the provided arguments.</summary>
        /// <param name="value1">The first string value.</param>
        /// <param name="value2">The second string value.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> option, when provided.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will execute the static <see cref="string.Compare(string?, string?)" /> or
        /// <see cref="string.Compare(string?, string?, StringComparison)"/> method based on the provided arguments.</returns>
        public static MethodCallExpression CreateCompareCallExpression(Expression value1, Expression value2, StringComparison? stringComparison)
        {
            _ = value1.WhenNotNull(nameof(value1));
            _ = value2.WhenNotNull(nameof(value2));

            return stringComparison.HasValue
                ? CreateStaticComparisonCallExpression(CompareStringComparisonMethodInfo, value1, value2, stringComparison.Value)
                : Expression.Call(CompareMethodInfo, value1, value2);
        }

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.Contains(string)" /> or
        /// <see cref="string.Contains(string, StringComparison)"/> method based on the provided arguments.</summary>
        /// <param name="instance">The string instance to call the method on.</param>
        /// <param name="value">The string value to be passed to the called method.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> option, when provided.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will execute the static <see cref="string.Contains(string)" /> or
        /// <see cref="string.Contains(string, StringComparison)"/> method based on the provided arguments.</returns>
        public static MethodCallExpression CreateContainsCallExpression(Expression instance, Expression value, StringComparison? stringComparison)
        {
            _ = instance.WhenNotNull(nameof(instance));
            _ = value.WhenNotNull(nameof(value));

            return stringComparison.HasValue
                ? CreateInstanceComparisonCallExpression(ContainsStringComparisonMethodInfo, instance, value, stringComparison.Value)
                : Expression.Call(instance, ContainsMethodInfo, value);
        }

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.StartsWith(string)" /> or
        /// <see cref="string.StartsWith(string, StringComparison)"/> method based on the provided arguments.</summary>
        /// <param name="instance">The string instance to call the method on.</param>
        /// <param name="value">The string value to be passed to the called method.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> option, when provided.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will execute the static <see cref="string.StartsWith(string)" /> or
        /// <see cref="string.StartsWith(string, StringComparison)"/> method based on the provided arguments.</returns>
        public static MethodCallExpression CreateStartsWithCallExpression(Expression instance, Expression value, StringComparison? stringComparison)
        {
            _ = instance.WhenNotNull(nameof(instance));
            _ = value.WhenNotNull(nameof(value));

            return stringComparison.HasValue
                ? CreateInstanceComparisonCallExpression(StartsWithStringComparisonMethodInfo, instance, value, stringComparison.Value)
                : Expression.Call(instance, StartsWithMethodInfo, value);
        }

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.EndsWith(string)" /> or
        /// <see cref="string.EndsWith(string, StringComparison)"/> method based on the provided arguments.</summary>
        /// <param name="instance">The string instance to call the method on.</param>
        /// <param name="value">The string value to be passed to the called method.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> option, when provided.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will execute the static <see cref="string.EndsWith(string)" /> or
        /// <see cref="string.EndsWith(string, StringComparison)"/> method based on the provided arguments.</returns>
        public static MethodCallExpression CreateEndsWithCallExpression(Expression instance, Expression value, StringComparison? stringComparison)
        {
            _ = instance.WhenNotNull(nameof(instance));
            _ = value.WhenNotNull(nameof(value));

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
