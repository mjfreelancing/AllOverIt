using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the hyperbolic tangent of a specified angle (in radians).
    public sealed class AoiTanhOperator : AoiUnaryOperator
    {
        public AoiTanhOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Tanh", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}