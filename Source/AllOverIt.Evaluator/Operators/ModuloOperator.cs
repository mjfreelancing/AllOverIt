using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the modulo of a given operand.
    public sealed class ModuloOperator : BinaryOperator
    {
        public ModuloOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Modulo, leftOperand, rightOperand)
        {
        }
    }
}