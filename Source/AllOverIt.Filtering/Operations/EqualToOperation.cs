﻿using AllOverIt.Filtering.Builders;
using AllOverIt.Filtering.Options;
using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class EqualToOperation<TEntity, TProperty> : OperationBase<TEntity, TProperty> where TEntity : class
    {
        public EqualToOperation(Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value, IOperationFilterOptions options)
            : base(propertyExpression, value, true, CreatePredicate, options)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant)
        {
            return SystemExpression.Equal(member, constant);
        }
    }
}