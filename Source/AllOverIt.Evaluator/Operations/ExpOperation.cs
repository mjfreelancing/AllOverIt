using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to raise 'e' to a specified power.</summary>
    public sealed class ExpOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public ExpOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new ExpOperator(e[0]));
        }
    }
}