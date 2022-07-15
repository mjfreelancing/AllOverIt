using System;
using System.Linq.Expressions;
using System.Reflection;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class StartsWith<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        private static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });

        public StartsWith(Expression<Func<TEntity, string>> propertyExpression, string value)
            : base(propertyExpression, value, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, ConstantExpression constant)
        {
            return SystemExpression.Call(member, StartsWithMethod, constant);
        }
    }
}