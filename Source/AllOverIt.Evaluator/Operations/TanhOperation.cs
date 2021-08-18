using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the hyperbolic tangent of an angle (in radians).</summary>
    public sealed class TanhOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public TanhOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new TanhOperator(e[0]));
        }
    }
}