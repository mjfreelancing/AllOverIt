using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that compares if one operand is less than a second.</summary>
    internal sealed class LessThanOperator : BinaryOperator
    {
        public LessThanOperator(Expression leftOperand, Expression rightOperand)
            : base(CreateExpression, leftOperand, rightOperand)
        {
        }

        private static Expression CreateExpression(Expression leftOperand, Expression rightOperand)
        {
            return Expression.LessThan(leftOperand, rightOperand);
        }
    }
}