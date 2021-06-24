using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the hyperbolic cosine of an angle (in radians).
    public sealed class CoshOperation : ArithmeticOperationBase
    {
        public CoshOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new CoshOperator(e[0]));
        }
    }
}