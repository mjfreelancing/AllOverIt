using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the smallest integral value greater than or equal to a given number.</summary>
    public sealed class CeilingOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public CeilingOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new CeilingOperator(e[0]));
        }
    }
}