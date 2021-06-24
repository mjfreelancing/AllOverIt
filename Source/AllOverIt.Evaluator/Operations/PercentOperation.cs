using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the percentage that one operand is of another.
    public sealed class PercentOperation : ArithmeticOperationBase
    {
        public PercentOperation()
            : base(2, MakeOperator)
        {
        }

        internal static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new PercentOperator(e[0], e[1]));
        }
    }
}