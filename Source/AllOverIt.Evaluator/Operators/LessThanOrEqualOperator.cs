using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that compares if one operand is less than or equal to a second.</summary>
    internal sealed class LessThanOrEqualOperator : BinaryOperator
    {
        public LessThanOrEqualOperator(Expression leftOperand, Expression rightOperand)
            : base(CreateExpression, leftOperand, rightOperand)
        {
        }

        private static Expression CreateExpression(Expression leftOperand, Expression rightOperand)
        {
            return Expression.LessThanOrEqual(leftOperand, rightOperand);
        }
    }
}