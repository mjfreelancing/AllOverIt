using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the square root of a number.
    public sealed class SqrtOperation : ArithmeticOperationBase
    {
        public SqrtOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new SqrtOperator(e[0]));
        }
    }
}