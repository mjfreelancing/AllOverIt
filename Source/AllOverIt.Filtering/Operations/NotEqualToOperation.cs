﻿using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class NotEqualToOperation<TEntity, TProperty> : OperationBase<TEntity, TProperty> where TEntity : class
    {
        public NotEqualToOperation(Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value)
            : base(propertyExpression, value, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant)
        {
            return SystemExpression.NotEqual(member, constant);
        }
    }
}