using AllOverIt.Filtering.Options;
using System.Linq.Expressions;
using System.Reflection;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal sealed class InOperation<TEntity, TProperty> : OperationBase<TEntity, TProperty> where TEntity : class
    {
        private static readonly MethodInfo ContainsMethod = typeof(ICollection<TProperty>).GetMethod("Contains", [typeof(TProperty)]);

        public InOperation(Expression<Func<TEntity, TProperty>> propertyExpression, IList<TProperty> values, IOperationFilterOptions options)
            : base(propertyExpression, values, options, (member, constant, _) => CreatePredicate(member, constant))
        {
        }

        private static MethodCallExpression CreatePredicate(SystemExpression member, SystemExpression constant)
        {
            return SystemExpression.Call(constant, ContainsMethod, member);
        }
    }
}