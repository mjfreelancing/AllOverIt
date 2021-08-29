using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the log2 of a number.</summary>
    public sealed class Log2Operation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public Log2Operation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new Log2Operator(e[0]));
        }
    }
}