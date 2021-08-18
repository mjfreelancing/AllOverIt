using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>Represents an arithmetic operator or user defined method.</summary>
    public interface IArithmeticOperation
    {
        /// <summary>The number of arguments required by the operator or user defined method.</summary>
        int ArgumentCount { get; }

        /// <summary>Creates an Expression that represents the operator or user defined method.</summary>
        /// <param name="expressions">The arguments required for creating the required expression.</param>
        /// <returns>The expression representing the operator or user defined method.</returns>
        Expression GetExpression(Expression[] expressions);
    }
}