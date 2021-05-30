using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the hyperbolic sine of a specified angle (in radians).
    public sealed class AoiSinhOperator : AoiUnaryOperator
    {
        public AoiSinhOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Sinh", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}