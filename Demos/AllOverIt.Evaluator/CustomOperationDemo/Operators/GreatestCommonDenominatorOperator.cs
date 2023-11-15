using AllOverIt.Evaluator.Operators;
using CustomOperationDemo.Math;
using System.Linq.Expressions;

namespace CustomOperationDemo.Operators
{
    public sealed class GreatestCommonDenominatorOperator : BinaryOperator
    {
        public GreatestCommonDenominatorOperator(Expression value1, Expression value2)
            : base(CreateExpression, value1, value2)
        {
        }

        private static Expression CreateExpression(Expression value1, Expression value2)
        {
            var method = typeof(CustomMath).GetMethod("GreatestCommonDenominator", [typeof(int), typeof(int)]);

            var val1 = Expression.Convert(value1, typeof(int));
            var val2 = Expression.Convert(value2, typeof(int));

            var result = Expression.Call(method!, val1, val2);

            return Expression.Convert(result, typeof(double));
        }
    }
}