using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that divides two operands.
    public sealed class DivideOperator : BinaryOperator
    {
        public DivideOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Divide, leftOperand, rightOperand)
        {
        }
    }
}