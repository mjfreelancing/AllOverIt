using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that calculates the percentage that one operand is of another.</summary>
    public sealed class PercentOperator : BinaryOperator
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftOperand">The left operand (argument) of the operator.</param>
        /// <param name="rightOperand">The right operand (argument) of the operator.</param>
        public PercentOperator(Expression leftOperand, Expression rightOperand)
            : base(CreateExpression, leftOperand, rightOperand)
        {
        }

        private static Expression CreateExpression(Expression leftOperand, Expression rightOperand)
        {
            var numerator = Expression.Multiply(leftOperand, Expression.Constant(100d));
            return Expression.Divide(numerator, rightOperand);
        }
    }
}