using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the sine of an angle (in radians).
    public sealed class AoiSinOperation : AoiArithmeticOperationBase
    {
        public AoiSinOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiSinOperator(e[0]));
        }
    }
}