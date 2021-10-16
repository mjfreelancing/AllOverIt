using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An operator that accepts a boolean operand and returns a 'true' or 'false' operand based on the condition's result.</summary>
    internal sealed class IfOperator : TernaryOperator
    {
        /// <summary>Constructor.</summary>
        /// <param name="condition">The boolean condition operand.</param>
        /// <param name="trueOperand">The operand used if the condition is true.</param>
        /// <param name="falseOperand">The operand used if the condition is false.</param>
        public IfOperator(Expression condition, Expression trueOperand, Expression falseOperand)
            : base(CreateExpression, condition, trueOperand, falseOperand)
        {
        }

        private static Expression CreateExpression(Expression condition, Expression trueOperand, Expression falseOperand)
        {
            return Expression.Condition(condition, trueOperand, falseOperand);
        }
    }
}