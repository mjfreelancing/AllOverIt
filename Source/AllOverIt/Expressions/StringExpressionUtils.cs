using AllOverIt.Assertion;
using AllOverIt.Reflection;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Expressions
{
    /// <summary>A utility class that provides expression helpers related string operations.</summary>
    public static class StringExpressionUtils
    {
        // Compare
        private static readonly MethodInfo CompareMethodInfo = CommonTypes.StringType.GetMethod(
            "Compare", BindingFlags.Public | BindingFlags.Static, null, new[] { CommonTypes.StringType, CommonTypes.StringType }, null);        
        
        private static readonly MethodInfo CompareStringComparisonMethodInfo = CommonTypes.StringType.GetMethod(
            "Compare", BindingFlags.Public | BindingFlags.Static, null, new[] { CommonTypes.StringType, CommonTypes.StringType, CommonTypes.StringComparisonType }, null);

        // Contains
        private static readonly MethodInfo ContainsMethodInfo = CommonTypes.StringType.GetMethod(
            "Contains", new[] { CommonTypes.StringType });

        private static readonly MethodInfo ContainsStringComparisonMethodInfo = CommonTypes.StringType.GetMethod(
            "Contains", new[] { CommonTypes.StringType, CommonTypes.StringComparisonType });

        // StartsWith
        private static readonly MethodInfo StartsWithMethodInfo = CommonTypes.StringType.GetMethod(
            "StartsWith", new[] { CommonTypes.StringType });

        private static readonly MethodInfo StartsWithStringComparisonMethodInfo = CommonTypes.StringType.GetMethod(
            "StartsWith", new[] { CommonTypes.StringType, CommonTypes.StringComparisonType });

        // EndsWith
        private static readonly MethodInfo EndsWithMethodInfo = CommonTypes.StringType.GetMethod(
            "EndsWith", new[] { CommonTypes.StringType });

        private static readonly MethodInfo EndsWithStringComparisonMethodInfo = CommonTypes.StringType.GetMethod(
            "EndsWith", new[] { CommonTypes.StringType, CommonTypes.StringComparisonType });

        private static readonly MethodInfo ToLowerMethodInfo = CommonTypes.StringType.GetMethod("ToLower", Type.EmptyTypes);

        private static readonly MethodInfo ToUpperMethodInfo = CommonTypes.StringType.GetMethod("ToUpper", Type.EmptyTypes);

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the static method <see cref="string.Compare(string?, string?)" /> or
        /// <see cref="string.Compare(string?, string?, StringComparison)"/> method based on the provided arguments.</summary>
        /// <param name="value1">The first string value.</param>
        /// <param name="value2">The second string value.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> option, when provided.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will execute the static <see cref="string.Compare(string?, string?)" /> or
        /// <see cref="string.Compare(string?, string?, StringComparison)"/> method based on the provided arguments.</returns>
        public static MethodCallExpression CreateCompareCallExpression(Expression value1, Expression value2, StringComparison? stringComparison = default)
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
        public static MethodCallExpression CreateContainsCallExpression(Expression instance, Expression value, StringComparison? stringComparison = default)
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
        public static MethodCallExpression CreateStartsWithCallExpression(Expression instance, Expression value, StringComparison? stringComparison = default)
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
        public static MethodCallExpression CreateEndsWithCallExpression(Expression instance, Expression value, StringComparison? stringComparison = default)
        {
            _ = instance.WhenNotNull(nameof(instance));
            _ = value.WhenNotNull(nameof(value));

            return stringComparison.HasValue
                ? CreateInstanceComparisonCallExpression(EndsWithStringComparisonMethodInfo, instance, value, stringComparison.Value)
                : Expression.Call(instance, EndsWithMethodInfo, value);
        }

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.ToLower" />.</summary>
        /// <param name="instance">The string instance to call the method on.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.ToLower" />.</returns>
        public static MethodCallExpression CreateToLowerCallExpression(Expression instance)
        {
            _ = instance.WhenNotNull(nameof(instance));

            return Expression.Call(instance, ToLowerMethodInfo);
        }

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.ToUpper" />.</summary>
        /// <param name="instance">The string instance to call the method on.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.ToUpper" />.</returns>
        public static MethodCallExpression CreateToUpperCallExpression(Expression instance)
        {
            _ = instance.WhenNotNull(nameof(instance));

            return Expression.Call(instance, ToUpperMethodInfo);
        }

        private static MethodCallExpression CreateStaticComparisonCallExpression(MethodInfo methodInfo, Expression instance, Expression value,
            StringComparison stringComparison)
        {
            var comparison = Expression.Constant(stringComparison);
            return Expression.Call(methodInfo, instance, value, comparison);
        }

        private static MethodCallExpression CreateInstanceComparisonCallExpression(MethodInfo methodInfo, Expression instance, Expression value,
            StringComparison stringComparison)
        {
            var comparison = Expression.Constant(stringComparison);
            return Expression.Call(instance, methodInfo, value, comparison);
        }
    }
}
