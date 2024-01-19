using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that calculates the cosine of a specified angle (in radians).</summary>
    public sealed class CosOperator : UnaryOperator
    {
        private static readonly MethodInfo OperatorMethodInfo = typeof(Math).GetMethod("Cos", [typeof(double)]);

        /// <summary>Constructor.</summary>
        /// <param name="operand">The operand (argument) to be evaluated.</param>
        public CosOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            return Expression.Call(OperatorMethodInfo, operand);
        }
    }
}