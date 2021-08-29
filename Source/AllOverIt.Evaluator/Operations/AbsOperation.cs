using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the absolute value of a number.</summary>
    public sealed class AbsOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public AbsOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new AbsOperator(e[0]));
        }
    }
}