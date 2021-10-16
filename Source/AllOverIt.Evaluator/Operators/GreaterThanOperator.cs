using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that compares if one operand is greater than a second.</summary>
    internal sealed class GreaterThanOperator : BinaryOperator
    {
        public GreaterThanOperator(Expression leftOperand, Expression rightOperand)
            : base(CreateExpression, leftOperand, rightOperand)
        {
        }

        private static Expression CreateExpression(Expression leftOperand, Expression rightOperand)
        {
            return Expression.GreaterThan(leftOperand, rightOperand);
        }
    }
}