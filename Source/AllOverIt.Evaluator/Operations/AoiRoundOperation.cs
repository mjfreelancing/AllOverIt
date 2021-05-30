using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to round a number to a specified number of decimal places.
    public sealed class AoiRoundOperation : AoiArithmeticOperationBase
    {
        public AoiRoundOperation()
            : base(2, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiRoundOperator(e[0], e[1]));
        }
    }
}
