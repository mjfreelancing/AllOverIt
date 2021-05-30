using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the sine of a specified angle (in radians).
    public sealed class AoiSinOperator : AoiUnaryOperator
    {
        public AoiSinOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Sin", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}