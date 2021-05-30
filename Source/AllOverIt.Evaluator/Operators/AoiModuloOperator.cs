using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the modulo of a given operand.
    public sealed class AoiModuloOperator : AoiBinaryOperator
    {
        public AoiModuloOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Modulo, leftOperand, rightOperand)
        {
        }
    }
}