using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that compares two operands for inequality.</summary>
    internal sealed class NotEqualOperator : BinaryOperator
    {
        public NotEqualOperator(Expression leftOperand, Expression rightOperand)
            : base(CreateExpression, leftOperand, rightOperand)
        {
        }

        private static Expression CreateExpression(Expression leftOperand, Expression rightOperand)
        {
            return Expression.NotEqual(leftOperand, rightOperand);
        }
    }
}