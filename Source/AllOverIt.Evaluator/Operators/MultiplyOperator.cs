using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that multiplies two operands.</summary>
    public sealed class MultiplyOperator : BinaryOperator
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftOperand">The left operand (argument) of the operator.</param>
        /// <param name="rightOperand">The right operand (argument) of the operator.</param>
        public MultiplyOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Multiply, leftOperand, rightOperand)
        {
        }
    }
}