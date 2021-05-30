using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the hyperbolic cosine of an angle (in radians).
    public sealed class AoiCoshOperation : AoiArithmeticOperationBase
    {
        public AoiCoshOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiCoshOperator(e[0]));
        }
    }
}