using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation that returns <see langword="True" /> if one operand is less than a second, otherwise <see langword="False" />.</summary>
    public sealed class LessThanOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public LessThanOperation()
            : base(2, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new LessThanOperator(e[0], e[1]));
        }
    }
}