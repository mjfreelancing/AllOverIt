using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that negates the value of a given operand.</summary>
    public sealed class NegateOperator : UnaryOperator
    {
        /// <summary>Constructor.</summary>
        /// <param name="operand">The operand (argument) to be evaluated.</param>
        public NegateOperator(Expression operand)
            : base(Expression.Negate, operand)
        {
        }
    }
}