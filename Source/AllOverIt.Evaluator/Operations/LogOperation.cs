using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the log10 of a number.
    public sealed class LogOperation : ArithmeticOperationBase
    {
        public LogOperation()
            : base(1, MakeOperator)
        {
        }

        internal static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new LogOperator(e[0]));
        }
    }
}