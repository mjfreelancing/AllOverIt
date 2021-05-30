using AllOverIt.Evaluator.Operators;
using CustomOperation.Math;
using System.Linq.Expressions;

namespace CustomOperation.Operators
{
    public sealed class GreatestCommonDenominatorOperator : AoiBinaryOperator
    {
        public GreatestCommonDenominatorOperator(Expression value1, Expression value2)
            : base(CreateExpression, value1, value2)
        {
        }

        private static Expression CreateExpression(Expression value1, Expression value2)
        {
            var method = typeof(CustomMath).GetMethod("GreatestCommonDenominator", new[] { typeof(int), typeof(int) });

            var val1 = Expression.Convert(value1, typeof(int));
            var val2 = Expression.Convert(value2, typeof(int));

            var result = Expression.Call(method, val1, val2);

            return Expression.Convert(result, typeof(double));
        }
    }
}