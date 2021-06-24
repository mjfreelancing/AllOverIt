using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the angle (in radians) of a tangent value.
    public sealed class AtanOperator : UnaryOperator
    {
        public AtanOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Atan", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}