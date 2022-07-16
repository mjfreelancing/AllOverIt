using AllOverIt.Filtering.Extensions;
using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class LessThanOperation<TEntity, TProperty> : OperationBase<TEntity, TProperty> where TEntity : class
    {
        public LessThanOperation(Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value, IFilterSpecificationBuilderOptions options)
            : base(propertyExpression, value, CreatePredicate, options)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant)
        {
            return SystemExpression.LessThan(member, constant);
        }
    }
}