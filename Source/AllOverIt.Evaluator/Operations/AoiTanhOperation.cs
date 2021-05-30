using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the hyperbolic tangent of an angle (in radians).
    public sealed class AoiTanhOperation : AoiArithmeticOperationBase
    {
        public AoiTanhOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiTanhOperator(e[0]));
        }
    }
}