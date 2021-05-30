using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the angle (in radians) of a cosine value.
    public sealed class AoiAcosOperator : AoiUnaryOperator
    {
        public AoiAcosOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Acos", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}