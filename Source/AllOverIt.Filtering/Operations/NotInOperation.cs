using AllOverIt.Filtering.Options;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class NotInOperation<TEntity, TProperty> : OperationBase<TEntity, TProperty> where TEntity : class
    {
        private static readonly MethodInfo ContainsMethod = typeof(ICollection<TProperty>).GetMethod("Contains", new[] { typeof(TProperty) });

        public NotInOperation(Expression<Func<TEntity, TProperty>> propertyExpression, IList<TProperty> values, IOperationFilterOptions options)
            : base(propertyExpression, values, CreatePredicate, options)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant)
        {
            var contains = SystemExpression.Call(constant, ContainsMethod, member);

            return SystemExpression.Not(contains);
        }
    }
}