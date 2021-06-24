using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to raise 'e' to a specified power.
    public sealed class ExpOperation : ArithmeticOperationBase
    {
        public ExpOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new ExpOperator(e[0]));
        }
    }
}