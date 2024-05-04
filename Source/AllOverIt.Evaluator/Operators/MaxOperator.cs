using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that returns the maximum of two values.</summary>
    public sealed class MaxOperator : BinaryOperator
    {
        private static readonly MethodInfo OperatorMethodInfo = typeof(Math).GetMethod("Max", [typeof(double), typeof(double)]);

        /// <summary>Constructor.</summary>
        /// <param name="leftOperand">The left operand (argument) of the operator.</param>
        /// <param name="rightOperand">The right operand (argument) of the operator.</param>
        public MaxOperator(Expression leftOperand, Expression rightOperand)
            : base(CreateExpression, leftOperand, rightOperand)
        {
        }

        private static Expression CreateExpression(Expression leftOperand, Expression rightOperand)
        {
            return Expression.Call(OperatorMethodInfo, leftOperand, rightOperand);
        }
    }
}