using AllOverIt.Filtering.Builders;
using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class EndsWithOperation<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        public EndsWithOperation(Expression<Func<TEntity, string>> propertyExpression, string value, IFilterSpecificationBuilderOptions options)
            : base(propertyExpression, value, false, (member, constant) => CreatePredicate(member, constant, options.StringComparison), options)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant, StringComparison? stringComparison)
        {
            if (stringComparison.HasValue)
            {
                var comparison = SystemExpression.Constant(stringComparison.Value);
                return SystemExpression.Call(member, StringFilterMethodInfo.EndsWithStringComparison, constant, comparison);
            }

            return SystemExpression.Call(member, StringFilterMethodInfo.EndsWith, constant);
        }
    }
}