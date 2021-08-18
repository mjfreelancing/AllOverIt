using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the tangent of an angle (in radians).</summary>
    public sealed class TanOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public TanOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new TanOperator(e[0]));
        }
    }
}