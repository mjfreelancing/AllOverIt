using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the angle (in radians) of a cosine value.
    public sealed class AoiAcosOperation : AoiArithmeticOperationBase
    {
        public AoiAcosOperation() 
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiAcosOperator(e[0]));
        }
    }
}