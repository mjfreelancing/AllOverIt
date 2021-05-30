using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the natural log of a number.
    public sealed class AoiLnOperation : AoiArithmeticOperationBase
    {
        public AoiLnOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiLnOperator(e[0]));
        }
    }
}