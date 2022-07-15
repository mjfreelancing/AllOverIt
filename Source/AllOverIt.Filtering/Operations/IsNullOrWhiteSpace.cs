using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class IsNullOrWhiteSpace<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        private static readonly SystemExpression NullConstant = SystemExpression.Constant(null);
        private static readonly SystemExpression EmptyConstant = SystemExpression.Constant(string.Empty);

        public IsNullOrWhiteSpace(Expression<Func<TEntity, string>> propertyExpression)
            : base(propertyExpression, (string)default, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, ConstantExpression _)
        {
            var isNull = SystemExpression.Equal(member, NullConstant);

            // TODO : Need to Trim() for the whitespace check, which means the Not Null is required
            var isEmpty = SystemExpression.AndAlso(
                    SystemExpression.NotEqual(member, NullConstant),
                    SystemExpression.Equal(member, EmptyConstant));

            return SystemExpression.OrElse(isNull, isEmpty);
        }
    }
}