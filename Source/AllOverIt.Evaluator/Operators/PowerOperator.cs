using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that calculates the value of one operand raised to the power of another.</summary>
    public sealed class PowerOperator : BinaryOperator
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftOperand">The left operand (argument) of the operator.</param>
        /// <param name="rightOperand">The right operand (argument) of the operator.</param>
        public PowerOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Power, leftOperand, rightOperand)
        {
        }
    }
}