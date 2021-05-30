using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that multiplies two operands.
    public sealed class AoiMultiplyOperator : AoiBinaryOperator
    {
        public AoiMultiplyOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Multiply, leftOperand, rightOperand)
        {
        }
    }
}