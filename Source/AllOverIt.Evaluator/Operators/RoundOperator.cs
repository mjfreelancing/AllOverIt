using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that rounds the value of one operand to the number of decimal places provided by another.</summary>
    public sealed class RoundOperator : BinaryOperator
    {
        private static readonly MethodInfo OperatorMethodInfo = typeof(Math).GetMethod("Round", [typeof(double), typeof(int), typeof(MidpointRounding)])!;
        private static readonly ConstantExpression MidpointRoundingExpression = Expression.Constant(MidpointRounding.AwayFromZero);

        /// <summary>Constructor.</summary>
        /// <param name="operand">The operand (argument) to be rounded.</param>
        /// <param name="decimals">The number of decimal places to round the operand to.</param>
        public RoundOperator(Expression operand, Expression decimals)
            : base(CreateExpression, operand, decimals)
        {
        }

        private static Expression CreateExpression(Expression operand, Expression decimals)
        {
            var decimalPlaces = Expression.Convert(decimals, typeof(int));
            return Expression.Call(OperatorMethodInfo, operand, decimalPlaces, MidpointRoundingExpression);
        }
    }
}