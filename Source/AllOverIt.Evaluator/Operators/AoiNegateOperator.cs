using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that negates the value of a given operand.
    public sealed class AoiNegateOperator : AoiUnaryOperator
    {
        public AoiNegateOperator(Expression operand)
            : base(Expression.Negate, operand)
        {
        }
    }
}