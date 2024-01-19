using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that calculates the angle (in radians) of a sine value.</summary>
    public sealed class AsinOperator : UnaryOperator
    {
        private static readonly MethodInfo OperatorMethodInfo = typeof(Math).GetMethod("Asin", [typeof(double)]);

        /// <summary>Constructor.</summary>
        /// <param name="operand">The operand (argument) to be evaluated.</param>
        public AsinOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            return Expression.Call(OperatorMethodInfo, operand);
        }
    }
}