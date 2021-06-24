using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using CustomOperation.Operators;
using System.Linq.Expressions;

namespace CustomOperation.Operations
{
    public sealed class GreatestCommonDenominatorOperation : ArithmeticOperationBase
    {
        public GreatestCommonDenominatorOperation()
            : base(2, MakeOperator)
        {
        }

        public static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new GreatestCommonDenominatorOperator(e[0], e[1]));
        }
    }
}