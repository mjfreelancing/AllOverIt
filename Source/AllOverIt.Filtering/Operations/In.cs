using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class In<TEntity, TProperty> : OperationBase<TEntity, TProperty> where TEntity : class
    {
        private static readonly MethodInfo ContainsMethod = typeof(IList).GetMethod("Contains", new[] { typeof(TProperty) });

        public In(Expression<Func<TEntity, TProperty>> propertyExpression, IList<TProperty> values)
            : base(propertyExpression, values, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, ConstantExpression constant)
        {
            return SystemExpression.Call(constant, ContainsMethod, member);
        }
    }

}