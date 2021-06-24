using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the angle (in radians) of a sine value.
    public sealed class AsinOperator : UnaryOperator
    {
        public AsinOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Asin", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}