using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the square root of a number.</summary>
    public sealed class SqrtOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public SqrtOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new SqrtOperator(e[0]));
        }
    }
}