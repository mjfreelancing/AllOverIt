using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the log10 of a number.</summary>
    public sealed class LogOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public LogOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new LogOperator(e[0]));
        }
    }
}