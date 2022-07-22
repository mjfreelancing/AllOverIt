﻿using AllOverIt.Expressions;
using AllOverIt.Filtering.Options;
using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class EqualToOperation<TEntity, TProperty> : OperationBase<TEntity, TProperty> where TEntity : class
    {
        public EqualToOperation(Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value, IOperationFilterOptions options)
            : base(propertyExpression, value, !PropertyIsString, (member, constant) => CreatePredicate(member, constant, options.StringComparison), options)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant, StringComparison? stringComparison)
        {
            if (PropertyIsString)
            {
                var compareExpression = StringExpressionUtils.CreateCompareCallExpression(member, constant, stringComparison);
                return SystemExpression.Equal(compareExpression, ExpressionConstants.Zero);
            }

            return SystemExpression.Equal(member, constant);
        }
    }
}