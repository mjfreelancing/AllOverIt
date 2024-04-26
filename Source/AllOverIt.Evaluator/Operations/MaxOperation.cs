using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to get the maximum of two values.</summary>
    public sealed class MaxOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public MaxOperation()
            : base(2, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new MaxOperator(e[0], e[1]));
        }
    }
}