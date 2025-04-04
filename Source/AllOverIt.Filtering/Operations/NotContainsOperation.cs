﻿using AllOverIt.Expressions.Strings;
using AllOverIt.Filtering.Options;
using System.Linq.Expressions;

using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class NotContainsOperation<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        public NotContainsOperation(Expression<Func<TEntity, string?>> propertyExpression, string? value, IOperationFilterOptions options)
            : base(propertyExpression, value, false, options, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(SystemExpression member, SystemExpression constant, IOperationFilterOptions options)
        {
            var contains = StringComparisonUtils.CreateContainsCallExpression(member, constant, options.StringComparisonMode);

            return SystemExpression.Not(contains);
        }
    }
}