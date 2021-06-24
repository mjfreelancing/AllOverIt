using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An interface used to obtain an Expression for an evaluator operator.
    public interface IOperator
    {
        Expression GetExpression();
    }
}
