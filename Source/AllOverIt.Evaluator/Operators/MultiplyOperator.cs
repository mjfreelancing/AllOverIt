using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that multiplies two operands.
    public sealed class MultiplyOperator : BinaryOperator
    {
        public MultiplyOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Multiply, leftOperand, rightOperand)
        {
        }
    }
}