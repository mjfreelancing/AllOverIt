using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to get the minimum of two values.</summary>
    public sealed class MinOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public MinOperation()
            : base(2, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new MinOperator(e[0], e[1]));
        }
    }
}