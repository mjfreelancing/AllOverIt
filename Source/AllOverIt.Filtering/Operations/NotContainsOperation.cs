using AllOverIt.Filtering.Builders;
using AllOverIt.Filtering.Options;
using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class NotContainsOperation<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        public NotContainsOperation(Expression<Func<TEntity, string>> propertyExpression, string value, IOperationFilterOptions options)
            : base(propertyExpression, value, false, (member, constant) => CreatePredicate(member, constant, options.StringComparison), options)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant, StringComparison? stringComparison)
        {
            MethodCallExpression contains;

            if (stringComparison.HasValue)
            {
                var comparison = SystemExpression.Constant(stringComparison.Value);
                contains = SystemExpression.Call(member, StringFilterMethodInfo.ContainsStringComparison, constant, comparison);
            }
            else
            {
                contains = SystemExpression.Call(member, StringFilterMethodInfo.Contains, constant);
            }

            return SystemExpression.Not(contains);
        }
    }
}