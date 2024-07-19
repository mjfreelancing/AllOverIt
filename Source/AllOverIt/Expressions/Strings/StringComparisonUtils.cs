using AllOverIt.Assertion;
using AllOverIt.Expressions.Strings.Extensions;
using AllOverIt.Reflection;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Expressions.Strings
{
    /// <summary>A utility class that provides expression helpers related string operations.</summary>
    public static class StringComparisonUtils
    {
        private static readonly MethodInfo CompareMethodInfo = CommonTypes.StringType.GetMethod(
            "Compare", BindingFlags.Public | BindingFlags.Static, null, [CommonTypes.StringType, CommonTypes.StringType], null)!;

        private static readonly MethodInfo CompareStringComparisonMethodInfo = CommonTypes.StringType.GetMethod(
            "Compare", BindingFlags.Public | BindingFlags.Static, null, [CommonTypes.StringType, CommonTypes.StringType, CommonTypes.StringComparisonType], null)!;

        private static readonly MethodInfo ContainsMethodInfo = CommonTypes.StringType.GetMethod(
            "Contains", [CommonTypes.StringType])!;

        private static readonly MethodInfo ContainsStringComparisonMethodInfo = CommonTypes.StringType.GetMethod(
            "Contains", [CommonTypes.StringType, CommonTypes.StringComparisonType])!;

        private static readonly MethodInfo StartsWithMethodInfo = CommonTypes.StringType.GetMethod(
            "StartsWith", [CommonTypes.StringType])!;

        private static readonly MethodInfo StartsWithStringComparisonMethodInfo = CommonTypes.StringType.GetMethod(
            "StartsWith", [CommonTypes.StringType, CommonTypes.StringComparisonType])!;

        private static readonly MethodInfo EndsWithMethodInfo = CommonTypes.StringType.GetMethod(
            "EndsWith", [CommonTypes.StringType])!;

        private static readonly MethodInfo EndsWithStringComparisonMethodInfo = CommonTypes.StringType.GetMethod(
            "EndsWith", [CommonTypes.StringType, CommonTypes.StringComparisonType])!;

        private static readonly MethodInfo ToLowerMethodInfo = CommonTypes.StringType.GetMethod(
            "ToLower", Type.EmptyTypes)!;

        private static readonly MethodInfo ToUpperMethodInfo = CommonTypes.StringType.GetMethod(
            "ToUpper", Type.EmptyTypes)!;

        private static readonly Dictionary<StringComparisonMode, Func<Expression, Expression>> StringModifiers = new()
        {
            { StringComparisonMode.ToLower, CreateToLowerCallExpression },
            { StringComparisonMode.ToUpper, CreateToUpperCallExpression }
        };

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the static method <see cref="string.Compare(string, string)" /> or
        /// <see cref="string.Compare(string, string, StringComparison)"/> method based on the provided arguments.</summary>
        /// <param name="value1">The first string value.</param>
        /// <param name="value2">The second string value.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> option, when provided.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will execute the static <see cref="string.Compare(string, string)" /> or
        /// <see cref="string.Compare(string, string, StringComparison)"/> method based on the provided arguments.</returns>
        public static MethodCallExpression CreateCompareCallExpression(Expression value1, Expression value2, StringComparison? stringComparison = default)
        {
            _ = value1.WhenNotNull(nameof(value1));
            _ = value2.WhenNotNull(nameof(value2));

            value1 = ConvertIfNullConstant(value1);
            value2 = ConvertIfNullConstant(value2);

            return stringComparison.HasValue
                ? CreateStaticComparisonCallExpression(CompareStringComparisonMethodInfo, value1, value2, stringComparison.Value)
                : Expression.Call(CompareMethodInfo, value1, value2);
        }

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will perform a string comparison (equality) based on the provided
        /// <see cref="StringComparisonMode"/> option.</summary>
        /// <param name="value1">The first string value.</param>
        /// <param name="value2">The second string value.</param>
        /// <param name="stringComparisonMode">The <see cref="StringComparisonMode"/> option that determines how the string comparison will be performed.
        /// If the mode is a modifier (<see cref="StringComparisonMode.ToLower"/> or <see cref="StringComparisonMode.ToUpper"/>) then the provided
        /// string expressions will be transformed before performing the string comparison. The other string comparison modes, such as
        /// <see cref="StringComparisonMode.InvariantCultureIgnoreCase"/> will perform a string comparison using the equivalent <see cref="StringComparison"/>
        /// mode.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will perform a string comparison based on the provided
        /// <see cref="StringComparisonMode"/> option.</returns>
        public static MethodCallExpression CreateCompareCallExpression(Expression value1, Expression value2, StringComparisonMode stringComparisonMode)
        {
            _ = value1.WhenNotNull(nameof(value1));
            _ = value2.WhenNotNull(nameof(value2));
            _ = stringComparisonMode.WhenNotNull(nameof(stringComparisonMode));

            if (stringComparisonMode.IsStringModifier())
            {
                // Cannot perform upper or lower on a null value
                ThrowIfNullConstant(value1, nameof(value1));
                ThrowIfNullConstant(value2, nameof(value2));
            }
            else
            {
                value1 = ConvertIfNullConstant(value1);
                value2 = ConvertIfNullConstant(value2);
            }

            return CreateStringComparisonCallExpression(value1, value2, stringComparisonMode,
                (val1, val2) => CreateCompareCallExpression(val1, val2),
                (val1, val2, stringComparison) => CreateCompareCallExpression(val1, val2, stringComparison));
        }

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.Contains(string)" /> or
        /// its <see cref="StringComparison"/> overload.</summary>
        /// <param name="instance">The string instance to call the method on.</param>
        /// <param name="value">The string value to be passed to the called method.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> option, when provided.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will execute the static <see cref="string.Contains(string)" /> or
        /// its <see cref="StringComparison"/> overload.</returns>
        public static MethodCallExpression CreateContainsCallExpression(Expression instance, Expression value, StringComparison? stringComparison = default)
        {
            _ = instance.WhenNotNull(nameof(instance));
            _ = value.WhenNotNull(nameof(value));

            // The instance 'Contains' method does not allow null
            ThrowIfNullConstant(instance, nameof(instance));
            ThrowIfNullConstant(value, nameof(value));

            return stringComparison.HasValue
                ? CreateInstanceComparisonCallExpression(ContainsStringComparisonMethodInfo, instance, value, stringComparison.Value)
                : Expression.Call(instance, ContainsMethodInfo, value);
        }

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will will execute the instance method <see cref="string.Contains(string, StringComparison)" />
        /// based on the provided <see cref="StringComparisonMode"/> option.</summary>
        /// <param name="instance">The string instance to call the method on.</param>
        /// <param name="value">The string value to be passed to the called method.</param>
        /// <param name="stringComparisonMode">The <see cref="StringComparisonMode"/> option that determines how the string comparison will be performed.
        /// If the mode is a modifier (<see cref="StringComparisonMode.ToLower"/> or <see cref="StringComparisonMode.ToUpper"/>) then the provided
        /// string expressions will be transformed before performing the string comparison. The other string comparison modes, such as
        /// <see cref="StringComparisonMode.InvariantCultureIgnoreCase"/> will perform a string comparison using the equivalent <see cref="StringComparison"/>
        /// mode.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will perform a string comparison based on the provided <see cref="StringComparisonMode"/> option.</returns>
        public static MethodCallExpression CreateContainsCallExpression(Expression instance, Expression value, StringComparisonMode stringComparisonMode)
        {
            _ = instance.WhenNotNull(nameof(instance));
            _ = value.WhenNotNull(nameof(value));
            _ = stringComparisonMode.WhenNotNull(nameof(stringComparisonMode));

            // The instance 'Contains' method does not allow null
            ThrowIfNullConstant(instance, nameof(instance));
            ThrowIfNullConstant(value, nameof(value));

            return CreateStringComparisonCallExpression(instance, value, stringComparisonMode,
                (val1, val2) => CreateContainsCallExpression(val1, val2),
                (val1, val2, stringComparison) => CreateContainsCallExpression(val1, val2, stringComparison));
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

            // The instance 'StartsWith' method does not allow null
            ThrowIfNullConstant(instance, nameof(instance));
            ThrowIfNullConstant(value, nameof(value));

            return stringComparison.HasValue
                ? CreateInstanceComparisonCallExpression(StartsWithStringComparisonMethodInfo, instance, value, stringComparison.Value)
                : Expression.Call(instance, StartsWithMethodInfo, value);
        }

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.Contains(string, StringComparison)"/>
        /// based on the provided <see cref="StringComparisonMode"/> option.</summary>
        /// <param name="instance">The string instance to call the method on.</param>
        /// <param name="value">The string value to be passed to the called method.</param>
        /// <param name="stringComparisonMode">The <see cref="StringComparisonMode"/> option that determines how the string comparison will be performed.
        /// If the mode is a modifier (<see cref="StringComparisonMode.ToLower"/> or <see cref="StringComparisonMode.ToUpper"/>) then the provided
        /// string expressions will be transformed before performing the string comparison. The other string comparison modes, such as
        /// <see cref="StringComparisonMode.InvariantCultureIgnoreCase"/> will perform a string comparison using the equivalent <see cref="StringComparison"/>
        /// mode.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will perform a string comparison based on the provided
        /// <see cref="StringComparisonMode"/> option.</returns>
        public static MethodCallExpression CreateStartsWithCallExpression(Expression instance, Expression value, StringComparisonMode stringComparisonMode)
        {
            _ = instance.WhenNotNull(nameof(instance));
            _ = value.WhenNotNull(nameof(value));
            _ = stringComparisonMode.WhenNotNull(nameof(stringComparisonMode));

            // The instance 'StartsWith' method does not allow null
            ThrowIfNullConstant(instance, nameof(instance));
            ThrowIfNullConstant(value, nameof(value));

            return CreateStringComparisonCallExpression(instance, value, stringComparisonMode,
                (val1, val2) => CreateStartsWithCallExpression(val1, val2),
                (val1, val2, stringComparison) => CreateStartsWithCallExpression(val1, val2, stringComparison));
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

            // The instance 'EndsWith' method does not allow null
            ThrowIfNullConstant(instance, nameof(instance));
            ThrowIfNullConstant(value, nameof(value));

            return stringComparison.HasValue
                ? CreateInstanceComparisonCallExpression(EndsWithStringComparisonMethodInfo, instance, value, stringComparison.Value)
                : Expression.Call(instance, EndsWithMethodInfo, value);
        }

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.EndsWith(string, StringComparison)" />
        /// based on the provided <see cref="StringComparisonMode"/> option.</summary>
        /// <param name="instance">The first string value.</param>
        /// <param name="value">The second string value.</param>
        /// <param name="stringComparisonMode">The <see cref="StringComparisonMode"/> option that determines how the string comparison will be performed.
        /// If the mode is a modifier (<see cref="StringComparisonMode.ToLower"/> or <see cref="StringComparisonMode.ToUpper"/>) then the provided
        /// string expressions will be transformed before performing the string comparison. The other string comparison modes, such as
        /// <see cref="StringComparisonMode.InvariantCultureIgnoreCase"/> will perform a string comparison using the equivalent <see cref="StringComparison"/>
        /// mode.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will perform a string comparison based on the provided
        /// <see cref="StringComparisonMode"/> option.</returns>
        public static MethodCallExpression CreateEndsWithCallExpression(Expression instance, Expression value, StringComparisonMode stringComparisonMode)
        {
            _ = instance.WhenNotNull(nameof(instance));
            _ = value.WhenNotNull(nameof(value));
            _ = stringComparisonMode.WhenNotNull(nameof(stringComparisonMode));

            // The instance 'EndsWith' method does not allow null
            ThrowIfNullConstant(instance, nameof(instance));
            ThrowIfNullConstant(value, nameof(value));

            return CreateStringComparisonCallExpression(instance, value, stringComparisonMode,
                (val1, val2) => CreateEndsWithCallExpression(val1, val2),
                (val1, val2, stringComparison) => CreateEndsWithCallExpression(val1, val2, stringComparison));
        }

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.ToLower()" />.</summary>
        /// <param name="value">The string instance to call the method on.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.ToLower()" />.</returns>
        public static MethodCallExpression CreateToLowerCallExpression(Expression value)
        {
            _ = value.WhenNotNull(nameof(value));

            ThrowIfNullConstant(value, nameof(value));

            return Expression.Call(value, ToLowerMethodInfo);
        }

        /// <summary>Creates a <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.ToUpper()" />.</summary>
        /// <param name="value">The string instance to call the method on.</param>
        /// <returns>A <see cref="MethodCallExpression"/> that will execute the instance method <see cref="string.ToUpper()" />.</returns>
        public static MethodCallExpression CreateToUpperCallExpression(Expression value)
        {
            _ = value.WhenNotNull(nameof(value));

            ThrowIfNullConstant(value, nameof(value));

            return Expression.Call(value, ToUpperMethodInfo);
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

        private static MethodCallExpression CreateStringComparisonCallExpression(Expression value1, Expression value2,
            StringComparisonMode stringComparisonMode,
            Func<Expression, Expression, MethodCallExpression> nonStringComparisonMethod,
            Func<Expression, Expression, StringComparison, MethodCallExpression> stringComparisonMethod)
        {
            if (stringComparisonMode.IsStringComparison())
            {
                var stringComparison = stringComparisonMode.GetStringComparison();

                return stringComparisonMethod.Invoke(value1, value2, stringComparison);
            }

            if (stringComparisonMode.IsStringModifier())
            {
                var modifier = StringModifiers[stringComparisonMode];

                value1 = modifier.Invoke(value1);
                value2 = modifier.Invoke(value2);
            }

            return nonStringComparisonMethod.Invoke(value1, value2);
        }

        private static Expression ConvertIfNullConstant(Expression value)
        {
            if (value is ConstantExpression constant && constant.Value is null && value.Type == CommonTypes.ObjectType)
            {
                value = Expression.Convert(value, typeof(string));
            }

            return value;
        }

        private static void ThrowIfNullConstant(Expression value, string parameterName)
        {
            // Check for Constant(null)
            var valueIsNull = value is ConstantExpression constant && constant.Value is null;

            Throw<ArgumentNullException>.When(valueIsNull, parameterName, "Expected a non-null expression value.");

            //// Check for Convert(Constant(null))
            //if (value is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
            //{
            //    ThrowIfNullConstant(unaryExpression.Operand, parameterName);
            //}
        }
    }
}
