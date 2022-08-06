using AllOverIt.Expressions;
using AllOverIt.Filtering.Extensions;
using AllOverIt.Filtering.Options;
using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class StartsWithOperation<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        public StartsWithOperation(Expression<Func<TEntity, string>> propertyExpression, string value, IOperationFilterOptions options)
            : base(propertyExpression, value, true, options, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(SystemExpression instance, SystemExpression value, IOperationFilterOptions options)
        {
            var stringComparisonMode = options.StringComparisonMode;

            if (stringComparisonMode.IsStringComparison())
            {
                var stringComparison = stringComparisonMode.GetStringComparison();

                return StringExpressionUtils.CreateStartsWithCallExpression(instance, value, stringComparison);
            }

            instance = instance.ApplyStringComparisonMode(stringComparisonMode);
            value = value.ApplyStringComparisonMode(stringComparisonMode);

            return StringExpressionUtils.CreateStartsWithCallExpression(instance, value);
        }
    }
}