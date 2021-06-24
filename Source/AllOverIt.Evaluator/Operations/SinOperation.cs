using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the sine of an angle (in radians).
    public sealed class SinOperation : ArithmeticOperationBase
    {
        public SinOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new SinOperator(e[0]));
        }
    }
}