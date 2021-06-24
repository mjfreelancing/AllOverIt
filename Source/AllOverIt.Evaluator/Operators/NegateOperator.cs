using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that negates the value of a given operand.
    public sealed class NegateOperator : UnaryOperator
    {
        public NegateOperator(Expression operand)
            : base(Expression.Negate, operand)
        {
        }
    }
}