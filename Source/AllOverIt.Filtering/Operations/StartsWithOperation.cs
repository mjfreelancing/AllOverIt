using AllOverIt.Filtering.Builders;
using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class StartsWithOperation<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        public StartsWithOperation(Expression<Func<TEntity, string>> propertyExpression, string value, IFilterSpecificationBuilderOptions options)
            : base(propertyExpression, value, true, (member, constant) => CreatePredicate(member, constant, options.StringComparison), options)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant, StringComparison? stringComparison)
        {
            if (stringComparison.HasValue)
            {
                var comparison = SystemExpression.Constant(stringComparison.Value);
                return SystemExpression.Call(member, StringFilterMethodInfo.StartsWithStringComparison, constant, comparison);
            }

            return SystemExpression.Call(member, StringFilterMethodInfo.StartsWith, constant);
        }
    }
}