using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the square root of a number.
    public sealed class AoiSqrtOperation : AoiArithmeticOperationBase
    {
        public AoiSqrtOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiSqrtOperator(e[0]));
        }
    }
}