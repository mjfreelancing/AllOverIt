using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the cosine of an angle (in radians).
    public sealed class AoiCosOperation : AoiArithmeticOperationBase
    {
        public AoiCosOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiCosOperator(e[0]));
        }
    }
}