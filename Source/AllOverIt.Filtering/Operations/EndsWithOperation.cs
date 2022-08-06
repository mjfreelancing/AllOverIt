using AllOverIt.Expressions;
using AllOverIt.Filtering.Extensions;
using AllOverIt.Filtering.Options;
using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class EndsWithOperation<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        public EndsWithOperation(Expression<Func<TEntity, string>> propertyExpression, string value, IOperationFilterOptions options)
            : base(propertyExpression, value, false, options, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(SystemExpression instance, SystemExpression value, IOperationFilterOptions options)
        {
            var stringComparisonMode = options.StringComparisonMode;

            if (stringComparisonMode.IsStringComparison())
            {
                var stringComparison = stringComparisonMode.GetStringComparison();

                return StringExpressionUtils.CreateEndsWithCallExpression(instance, value, stringComparison);
            }

            instance = instance.ApplyStringComparisonMode(stringComparisonMode);
            value = value.ApplyStringComparisonMode(stringComparisonMode);

            return StringExpressionUtils.CreateEndsWithCallExpression(instance, value);
        }
    }
}