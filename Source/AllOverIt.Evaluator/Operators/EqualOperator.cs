using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that compares two operands for equality.</summary>
    internal sealed class EqualOperator : BinaryOperator
    {
        public EqualOperator(Expression leftOperand, Expression rightOperand)
            : base(CreateExpression, leftOperand, rightOperand)
        {
        }

        private static Expression CreateExpression(Expression leftOperand, Expression rightOperand)
        {
            return Expression.Equal(leftOperand, rightOperand);
        }
    }
}