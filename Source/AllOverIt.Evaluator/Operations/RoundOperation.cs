using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to round a number to a specified number of decimal places.
    public sealed class RoundOperation : ArithmeticOperationBase
    {
        public RoundOperation()
            : base(2, MakeOperator)
        {
        }

        internal static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new RoundOperator(e[0], e[1]));
        }
    }
}
