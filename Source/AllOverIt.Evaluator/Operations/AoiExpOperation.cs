using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to raise 'e' to a specified power.
    public sealed class AoiExpOperation : AoiArithmeticOperationBase
    {
        public AoiExpOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiExpOperator(e[0]));
        }
    }
}