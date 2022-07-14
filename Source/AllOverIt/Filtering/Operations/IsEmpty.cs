using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    public sealed class IsEmpty<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        public IsEmpty(Expression<Func<TEntity, string>> propertyExpression)
            : base(propertyExpression, string.Empty, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, ConstantExpression constant)
        {
            return SystemExpression.Equal(member, constant);
        }
    }
}