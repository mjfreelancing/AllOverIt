using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the angle (in radians) of a tangent value.
    public sealed class AtanOperation : ArithmeticOperationBase
    {
        public AtanOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new AtanOperator(e[0]));
        }
    }
}