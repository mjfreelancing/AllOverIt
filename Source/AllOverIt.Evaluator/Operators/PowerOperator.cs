using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the value of one operand raised to the power of another.
    public sealed class PowerOperator : BinaryOperator
    {
        public PowerOperator(Expression leftOperand, Expression rightOperand)
            : base(Expression.Power, leftOperand, rightOperand)
        {
        }
    }
}