using AllOverIt.Expressions;
using AllOverIt.Expressions.Strings;
using AllOverIt.Filtering.Options;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class LessThanOrEqualOperation<TEntity, TProperty> : OperationBase<TEntity, TProperty> where TEntity : class
    {
        public LessThanOrEqualOperation(Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value, IOperationFilterOptions options)
            : base(propertyExpression, value, !PropertyIsString, options, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(SystemExpression member, SystemExpression constant, IOperationFilterOptions options)
        {
            if (PropertyIsString)
            {
                var compareExpression = StringComparisonUtils.CreateCompareCallExpression(member, constant, options.StringComparisonMode);

                return SystemExpression.LessThanOrEqual(compareExpression, ExpressionConstants.Zero);
            }

            return SystemExpression.LessThanOrEqual(member, constant);
        }
    }
}