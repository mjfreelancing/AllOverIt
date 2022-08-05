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

            return stringComparisonMode switch
            {
                StringComparisonMode.None => expression,
                StringComparisonMode.ToLower => StringExpressionUtils.CreateToLowerCallExpression(expression),
                StringComparisonMode.ToUpper => StringExpressionUtils.CreateToUpperCallExpression(expression),
                _ => throw new InvalidOperationException($"Unknown string comparison mode '{stringComparisonMode}'.")
            };
        }
    }
}