using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the hyperbolic cosine of an angle (in radians).</summary>
    public sealed class CoshOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public CoshOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new CoshOperator(e[0]));
        }
    }
}