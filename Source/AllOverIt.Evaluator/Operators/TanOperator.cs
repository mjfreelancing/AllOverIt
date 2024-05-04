using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that calculates the tangent of a specified angle (in radians).</summary>
    public sealed class TanOperator : UnaryOperator
    {
        private static readonly MethodInfo OperatorMethodInfo = typeof(Math).GetMethod("Tan", [typeof(double)]);

        /// <summary>Constructor.</summary>
        /// <param name="operand">The operand (argument) to be evaluated.</param>
        public TanOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            return Expression.Call(OperatorMethodInfo, operand);
        }
    }
}