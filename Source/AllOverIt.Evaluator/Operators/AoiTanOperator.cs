using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the tangent of a specified angle (in radians).
    public sealed class AoiTanOperator : AoiUnaryOperator
    {
        public AoiTanOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Tan", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}