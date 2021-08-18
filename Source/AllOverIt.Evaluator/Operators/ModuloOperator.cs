using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that calculates the modulo of a given operand.</summary>
    public sealed class ModuloOperator : BinaryOperator
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftOperand">The left operand (argument) of the operator.</param>
        /// <param name="rightOperand">The right operand (argument) of the operator.</param>
        public ModuloOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Modulo, leftOperand, rightOperand)
        {
        }
    }
}