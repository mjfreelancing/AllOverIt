using AllOverIt.Filtering.Builders;
using AllOverIt.Filtering.Options;
using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class ContainsOperation<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        public ContainsOperation(Expression<Func<TEntity, string>> propertyExpression, string value, IOperationFilterOptions options)
            : base(propertyExpression, value, false, (member, constant) => CreatePredicate(member, constant, options.StringComparison), options)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant, StringComparison? stringComparison)
        {
            if (stringComparison.HasValue)
            {
                var comparison = SystemExpression.Constant(stringComparison.Value);
                return SystemExpression.Call(member, StringFilterMethodInfo.ContainsStringComparison, constant, comparison);
            }

            return SystemExpression.Call(member, StringFilterMethodInfo.Contains, constant);
        }
    }
}