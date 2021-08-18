using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An interface used to obtain an Expression for an evaluator operator.</summary>
    public interface IOperator
    {
        /// <summary>Gets an Expression that is the result of invoking the operator.</summary>
        Expression GetExpression();
    }
}
