using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    public sealed class IsNull<TEntity, TProperty> : OperationBase<TEntity, TProperty>
        where TEntity : class
        where TProperty : class
    {
        public IsNull(Expression<Func<TEntity, TProperty>> propertyExpression)
            : base(propertyExpression, (TProperty) default, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, ConstantExpression constant)
        {
            return SystemExpression.Equal(member, constant);
        }
    }
}