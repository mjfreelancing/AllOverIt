﻿using System;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class IsNotEmpty<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        public IsNotEmpty(Expression<Func<TEntity, string>> propertyExpression)
            : base(propertyExpression, string.Empty, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, ConstantExpression constant)
        {
            return SystemExpression.NotEqual(member, constant);
        }
    }
}