using AllOverIt.Expressions;
using AllOverIt.Filtering.Extensions;
using AllOverIt.Filtering.Options;
using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class EndsWithOperation<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        public EndsWithOperation(Expression<Func<TEntity, string>> propertyExpression, string value, IOperationFilterOptions options)
            : base(propertyExpression, value, false, options, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(SystemExpression member, SystemExpression constant, IOperationFilterOptions filterOptions)
{
            member = member.ApplyStringComparisonMode(filterOptions.StringComparisonMode);
            constant = constant.ApplyStringComparisonMode(filterOptions.StringComparisonMode);

            return StringExpressionUtils.CreateEndsWithCallExpression(member, constant);
        }
    }
}