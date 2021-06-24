using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that subtracts one operand from another.
    public class SubtractOperator : BinaryOperator
    {
        public SubtractOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Subtract, leftOperand, rightOperand)
        {
        }
    }
}