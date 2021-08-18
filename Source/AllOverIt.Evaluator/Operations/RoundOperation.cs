using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to round a number to a specified number of decimal places.</summary>
    public sealed class RoundOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public RoundOperation()
            : base(2, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new RoundOperator(e[0], e[1]));
        }
    }
}
