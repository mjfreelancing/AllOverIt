using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the natural logarithm of a given operand.
    public sealed class LnOperator : UnaryOperator
    {
        public LnOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Log", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}