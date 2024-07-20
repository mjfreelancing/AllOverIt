using AllOverIt.Assertion;
using System.Linq.Expressions;

namespace AllOverIt.Expressions
{
    // Part of the predicate builder is based on:
    // https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/
    // https://docs.microsoft.com/en-us/archive/blogs/meek/linq-to-entities-combining-predicates

    /// <summary>A helper class that replaces parameter expressions in a lambda expression with alternate parameter expressions.</summary>
    public sealed class ParameterRebinder : ExpressionVisitor
    {
        private readonly IDictionary<ParameterExpression, ParameterExpression> _parameterMap;

        private ParameterRebinder(IDictionary<ParameterExpression, ParameterExpression> parameterMap)
        {
            _parameterMap = parameterMap;
        }

        /// <summary>Replaces parameter expressions in a lambda expression with alternate parameter expressions in the provided mapping.</summary>
        /// <param name="parameterMap">Contains the mapping of parameter expressions to replace in the provided lambda expression.</param>
        /// <param name="expression">The lambda expression to have its parameter expressions replaced.</param>
        /// <returns>A new expression with the parameter expressions replaced.</returns>
        public static Expression ReplaceParameters(IDictionary<ParameterExpression, ParameterExpression> parameterMap, Expression expression)
        {
            _ = parameterMap.WhenNotNull();
            _ = expression.WhenNotNull();

            return new ParameterRebinder(parameterMap).Visit(expression);
        }

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            if (_parameterMap.TryGetValue(parameter, out var replacement))
            {
                parameter = replacement;
            }

            return base.VisitParameter(parameter);
        }
    }
}