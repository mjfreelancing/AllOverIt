using System;
using System.Linq.Expressions;
using System.Reflection;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class EndsWithOperation<TEntity> : OperationBase<TEntity, string> where TEntity : class
    {
        private static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        public EndsWithOperation(Expression<Func<TEntity, string>> propertyExpression, string value, bool useParameterizedQueries)
            : base(propertyExpression, value, CreatePredicate, useParameterizedQueries)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, SystemExpression constant)
        {
            return SystemExpression.Call(member, EndsWithMethod, constant);
        }
    }
}