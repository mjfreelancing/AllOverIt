using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the hyperbolic tangent of an angle (in radians).
    public sealed class TanhOperation : ArithmeticOperationBase
    {
        public TanhOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new TanhOperator(e[0]));
        }
    }
}