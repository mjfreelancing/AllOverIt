using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using CustomOperation.Operators;
using System.Linq.Expressions;

namespace CustomOperation.Operations
{
    public sealed class GreatestCommonDenominatorOperation : AoiArithmeticOperationBase
    {
        public GreatestCommonDenominatorOperation()
            : base(2, MakeOperator)
        {
        }

        public static IAoiOperator MakeOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new GreatestCommonDenominatorOperator(e[0], e[1]));
        }
    }
}