using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the percentage that one operand is of another.
    public sealed class AoiPercentOperator : AoiBinaryOperator
    {
        public AoiPercentOperator(Expression leftOperand, Expression rightOperand)
            : base(CreateExpression, leftOperand, rightOperand)
        {
        }

        private static Expression CreateExpression(Expression leftOperand, Expression rightOperand)
        {
            var numerator = Expression.Multiply(leftOperand, Expression.Constant(100.0));
            return Expression.Divide(numerator, rightOperand);
        }
    }
}