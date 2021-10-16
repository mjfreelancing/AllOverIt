using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation that returns true if one operand is greater than or equal to a second, otherwise false.</summary>
    public sealed class GreaterThanOrEqualOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public GreaterThanOrEqualOperation()
            : base(2, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new GreaterThanOrEqualOperator(e[0], e[1]));
        }
    }
}