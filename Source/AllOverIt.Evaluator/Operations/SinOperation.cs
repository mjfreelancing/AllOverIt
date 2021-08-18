using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the sine of an angle (in radians).</summary>
    public sealed class SinOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public SinOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new SinOperator(e[0]));
        }
    }
}