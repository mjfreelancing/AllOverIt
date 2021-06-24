using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that adds two operands.
    public sealed class AddOperator : BinaryOperator
    {
        public AddOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Add, leftOperand, rightOperand)
        {
        }
    }
}