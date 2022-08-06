using AllOverIt.Expressions;
using AllOverIt.Filtering.Options;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Extensions
{
    internal static class ExpressionExtensions
    {
        public static Expression ApplyStringComparisonMode(this Expression expression, StringComparisonMode stringComparisonMode)
        {
            if (expression is ConstantExpression constant && constant.Value == null)
            {
                return expression;
            }

            if (stringComparisonMode == StringComparisonMode.None)
            {
                return expression;
            }

            if (stringComparisonMode == StringComparisonMode.ToLower)
            {
                return StringExpressionUtils.CreateToLowerCallExpression(expression);
            }

            if (stringComparisonMode == StringComparisonMode.ToUpper)
            {
                return StringExpressionUtils.CreateToUpperCallExpression(expression);
            }

            throw new InvalidOperationException($"Unknown string comparison mode '{stringComparisonMode}'.");
        }
    }
}