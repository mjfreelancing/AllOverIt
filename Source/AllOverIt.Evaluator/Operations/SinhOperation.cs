using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the hyperbolic sine of an angle (in radians).
    public sealed class SinhOperation : ArithmeticOperationBase
    {
        public SinhOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new SinhOperator(e[0]));
        }
    }
}