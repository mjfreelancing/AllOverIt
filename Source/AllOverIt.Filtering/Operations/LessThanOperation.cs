using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class LessThanOperation<TEntity, TProperty> : OperationBase<TEntity, TProperty> where TEntity : class
    {
        public LessThanOperation(Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value, bool useParameterizedQueries)
            : base(propertyExpression, value, CreatePredicate, useParameterizedQueries)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant)
        {
            return SystemExpression.LessThan(member, constant);
        }
    }
}