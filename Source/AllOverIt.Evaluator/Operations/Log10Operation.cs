using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the log10 of a number.</summary>
    public sealed class Log10Operation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public Log10Operation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new Log10Operator(e[0]));
        }
    }
}