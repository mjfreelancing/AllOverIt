using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // Represents an arithmetic operator or user defined method.
    public interface IArithmeticOperation
    {
        // The number of arguments required by the operator or user defined method.
        int ArgumentCount { get; }

        // Creates an Expression that represents the operator or user defined method.
        Expression GetExpression(Expression[] expressions);
    }
}