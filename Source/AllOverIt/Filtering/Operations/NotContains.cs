﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    public sealed class NotContains<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        public NotContains(Expression<Func<TEntity, string>> propertyExpression, string value)
            : base(propertyExpression, value, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, ConstantExpression constant)
        {
            var contains = SystemExpression.Call(member, ContainsMethod, constant);

            return SystemExpression.Not(contains);
        }
    }
}