using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the angle (in radians) of a tangent value.
    public sealed class AoiAtanOperation : AoiArithmeticOperationBase
    {
        public AoiAtanOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiAtanOperator(e[0]));
        }
    }
}