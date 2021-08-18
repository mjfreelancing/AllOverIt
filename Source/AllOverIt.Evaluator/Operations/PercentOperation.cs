using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the percentage that one operand is of another.</summary>
    public sealed class PercentOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public PercentOperation()
            : base(2, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new PercentOperator(e[0], e[1]));
        }
    }
}