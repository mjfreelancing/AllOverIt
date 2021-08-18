using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the angle (in radians) of a sine value.</summary>
    public sealed class AsinOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public AsinOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new AsinOperator(e[0]));
        }
    }
}