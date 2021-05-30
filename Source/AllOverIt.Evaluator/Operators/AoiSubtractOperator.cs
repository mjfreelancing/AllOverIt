using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that subtracts one operand from another.
    public class AoiSubtractOperator : AoiBinaryOperator
    {
        public AoiSubtractOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Subtract, leftOperand, rightOperand)
        {
        }
    }
}