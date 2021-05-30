using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the hyperbolic sine of an angle (in radians).
    public sealed class AoiSinhOperation : AoiArithmeticOperationBase
    {
        public AoiSinhOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiSinhOperator(e[0]));
        }
    }
}