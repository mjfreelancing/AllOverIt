using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that calculates the value of 'e' raised to the power of a given operand.
    public sealed class AoiExpOperator : AoiUnaryOperator
    {
        public AoiExpOperator(Expression operand)
            : base(CreateExpression, operand)
        {
        }

        private static Expression CreateExpression(Expression operand)
        {
            var method = typeof(Math).GetMethod("Exp", new[] { typeof(double) });
            return Expression.Call(method!, operand);
        }
    }
}