using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates log10 of a given operand.
    public sealed class LogOperator : UnaryOperator
    {
        public LogOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Log10", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}