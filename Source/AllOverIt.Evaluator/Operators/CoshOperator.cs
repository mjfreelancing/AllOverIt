using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the hyperbolic cosine of a specified angle (in radians).
    public sealed class CoshOperator : UnaryOperator
    {
        public CoshOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Cosh", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}