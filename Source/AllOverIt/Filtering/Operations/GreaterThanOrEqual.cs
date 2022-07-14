﻿using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    public sealed class GreaterThanOrEqual<TEntity, TProperty> : OperationBase<TEntity, TProperty> where TEntity : class
    {
        public GreaterThanOrEqual(Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value)
            : base(propertyExpression, value, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, ConstantExpression constant)
        {
            return SystemExpression.GreaterThanOrEqual(member, constant);
        }
    }
}