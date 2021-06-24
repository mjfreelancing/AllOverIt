using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the cosine of a specified angle (in radians).
    public sealed class CosOperator : UnaryOperator
    {
        public CosOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Cos", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}