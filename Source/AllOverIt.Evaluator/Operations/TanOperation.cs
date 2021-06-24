using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the tangent of an angle (in radians).
    public sealed class TanOperation : ArithmeticOperationBase
    {
        public TanOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new TanOperator(e[0]));
        }
    }
}