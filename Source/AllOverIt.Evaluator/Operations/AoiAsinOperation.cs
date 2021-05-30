using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the angle (in radians) of a sine value.
    public sealed class AoiAsinOperation : AoiArithmeticOperationBase
    {
        public AoiAsinOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiAsinOperator(e[0]));
        }
    }
}