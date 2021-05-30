using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the tangent of an angle (in radians).
    public sealed class AoiTanOperation : AoiArithmeticOperationBase
    {
        public AoiTanOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiTanOperator(e[0]));
        }
    }
}