using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the natural log of a number.
    public sealed class LnOperation : ArithmeticOperationBase
    {
        public LnOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new LnOperator(e[0]));
        }
    }
}