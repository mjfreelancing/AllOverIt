using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that calculates the natural logarithm of a given operand.</summary>
    public sealed class LogOperator : UnaryOperator
    {
        private static readonly MethodInfo OperatorMethodInfo = typeof(Math).GetMethod("Log", [typeof(double)]);

        /// <summary>Constructor.</summary>
        /// <param name="operand">The operand (argument) to be evaluated.</param>
        public LogOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            return Expression.Call(OperatorMethodInfo, operand);
        }
    }
}