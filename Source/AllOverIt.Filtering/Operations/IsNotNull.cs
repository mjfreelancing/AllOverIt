using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class IsNotNull<TEntity, TProperty> : OperationBase<TEntity, TProperty>
        where TEntity : class
        where TProperty : class
    {
        public IsNotNull(Expression<Func<TEntity, TProperty>> propertyExpression)
            : base(propertyExpression, (TProperty) default, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant)
        {
            return SystemExpression.NotEqual(member, constant);
        }
    }
}