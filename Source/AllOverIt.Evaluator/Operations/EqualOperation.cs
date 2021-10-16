using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to compare two values for equality, returning true or false.</summary>
    /// <remarks>Can be used in combination with the <see cref="IfOperation"/>.</remarks>
    public sealed class EqualOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public EqualOperation()
            : base(2, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new EqualOperator(e[0], e[1]));
        }
    }
}