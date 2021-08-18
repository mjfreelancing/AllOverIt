using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the angle (in radians) of a tangent value.</summary>
    public sealed class AtanOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public AtanOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new AtanOperator(e[0]));
        }
    }
}