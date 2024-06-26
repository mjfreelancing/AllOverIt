using AllOverIt.Evaluator.Operators;
using CustomOperationDemo.Math;
using System.Linq.Expressions;

namespace CustomOperationDemo.Operators
{
    public sealed class CustomMinOperator : BinaryOperator
    {
        public CustomMinOperator(Expression value1, Expression value2)
            : base(CreateExpression, value1, value2)
        {
        }

        private static Expression CreateExpression(Expression value1, Expression value2)
        {
            var method = typeof(CustomMath).GetMethod("CustomMin", [typeof(double), typeof(double)]);
            return Expression.Call(method!, value1, value2);
        }
    }
}