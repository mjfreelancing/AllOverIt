using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the natural log of a number.</summary>
    public sealed class LnOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public LnOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new LnOperator(e[0]));
        }
    }
}