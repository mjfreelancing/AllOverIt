using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the percentage that one operand is of another.
    public sealed class AoiPercentOperation : AoiArithmeticOperationBase
    {
        public AoiPercentOperation()
            : base(2, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiPercentOperator(e[0], e[1]));
        }
    }
}