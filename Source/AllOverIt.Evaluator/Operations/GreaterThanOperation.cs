using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation that returns <see langword="True" /> if one operand is greater than a second, otherwise <see langword="False" />.</summary>
    public sealed class GreaterThanOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public GreaterThanOperation()
            : base(2, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new GreaterThanOperator(e[0], e[1]));
        }
    }
}