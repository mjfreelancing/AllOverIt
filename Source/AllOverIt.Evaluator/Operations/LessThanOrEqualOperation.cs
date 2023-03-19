using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation that returns <see langword="true" /> if one operand is less than or equal to a second, otherwise <see langword="false" />.</summary>
    public sealed class LessThanOrEqualOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public LessThanOrEqualOperation()
            : base(2, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new LessThanOrEqualOperator(e[0], e[1]));
        }
    }
}