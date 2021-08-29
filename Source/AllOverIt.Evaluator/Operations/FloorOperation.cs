using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the largest integral value greater than or equal to a given number.</summary>
    public sealed class FloorOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public FloorOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new FloorOperator(e[0]));
        }
    }
}