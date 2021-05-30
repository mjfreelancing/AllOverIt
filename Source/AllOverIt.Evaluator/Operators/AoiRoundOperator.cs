using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that rounds the value of one operand to the number of decimal places provided by another.
    public sealed class AoiRoundOperator : AoiBinaryOperator
    {
        public AoiRoundOperator(Expression operand, Expression decimals)
            : base(CreateExpression, operand, decimals)
        {
        }

        private static Expression CreateExpression(Expression operand, Expression decimals)
        {
            var method = typeof(Math).GetMethod("Round", new[] { typeof(double), typeof(int), typeof(MidpointRounding) });
            var decimalPlaces = Expression.Convert(decimals, typeof(int));
            var midpointRounding = Expression.Constant(MidpointRounding.AwayFromZero);

            return Expression.Call(method!, operand, decimalPlaces, midpointRounding);
        }
    }
}