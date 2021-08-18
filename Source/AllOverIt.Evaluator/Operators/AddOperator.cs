using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that adds two operands.</summary>
    public sealed class AddOperator : BinaryOperator
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftOperand">The left operand (argument) of the operator.</param>
        /// <param name="rightOperand">The right operand (argument) of the operator.</param>
        public AddOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Add, leftOperand, rightOperand)
        {
        }
    }
}