using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that subtracts one operand from another.</summary>
    public class SubtractOperator : BinaryOperator
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftOperand">The left operand (argument) of the operator.</param>
        /// <param name="rightOperand">The right operand (argument) of the operator.</param>
        public SubtractOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Subtract, leftOperand, rightOperand)
        {
        }
    }
}