using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that returns the minimum of two values.</summary>
    public sealed class MinOperator : BinaryOperator
    {
        private static readonly MethodInfo OperatorMethodInfo = typeof(Math).GetMethod("Min", new[] { typeof(double), typeof(double) });

        /// <summary>Constructor.</summary>
        /// <param name="leftOperand">The left operand (argument) of the operator.</param>
        /// <param name="rightOperand">The right operand (argument) of the operator.</param>
        public MinOperator(Expression leftOperand, Expression rightOperand)
            : base(CreateExpression, leftOperand, rightOperand)
        {
        }

        private static Expression CreateExpression(Expression leftOperand, Expression rightOperand)
        {
            return Expression.Call(OperatorMethodInfo, leftOperand, rightOperand);
        }
    }
}