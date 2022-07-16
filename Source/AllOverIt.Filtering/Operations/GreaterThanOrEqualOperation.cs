using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class GreaterThanOrEqualOperation<TEntity, TProperty> : OperationBase<TEntity, TProperty> where TEntity : class
    {
        public GreaterThanOrEqualOperation(Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value, bool useParameterizedQueries)
            : base(propertyExpression, value, CreatePredicate, useParameterizedQueries)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant)
        {
            return SystemExpression.GreaterThanOrEqual(member, constant);
        }
    }
}