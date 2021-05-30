using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the square root of a given operand.
    public sealed class AoiSqrtOperator : AoiUnaryOperator
    {
        public AoiSqrtOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Sqrt", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}