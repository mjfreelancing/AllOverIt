using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the log10 of a number.
    public sealed class AoiLogOperation : AoiArithmeticOperationBase
    {
        public AoiLogOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiLogOperator(e[0]));
        }
    }
}