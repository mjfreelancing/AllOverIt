using AllOverIt.Expressions.Strings;
using AllOverIt.Filtering.Options;
using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class StartsWithOperation<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        public StartsWithOperation(Expression<Func<TEntity, string>> propertyExpression, string value, IOperationFilterOptions options)
            : base(propertyExpression, value, false, options, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(SystemExpression instance, SystemExpression value, IOperationFilterOptions options)
        {
            return StringComparisonUtils.CreateStartsWithCallExpression(instance, value, options.StringComparisonMode);
        }
    }
}