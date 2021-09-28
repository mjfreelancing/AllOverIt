using AllOverIt.Helpers;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that operates on two operands.</summary>
    public abstract class BinaryOperator : Operator<Func<Expression, Expression, Expression>>
    {
        internal readonly Expression LeftOperand;
        internal readonly Expression RightOperand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorType"></param>
        /// <param name="leftOperand">The left operand (argument) of the operator.</param>
        /// <param name="rightOperand">The right operand (argument) of the operator.</param>
        protected BinaryOperator(Func<Expression, Expression, Expression> operatorType, Expression leftOperand, Expression rightOperand)
            : base(operatorType)
        {
            LeftOperand = leftOperand.WhenNotNull(nameof(leftOperand));
            RightOperand = rightOperand.WhenNotNull(nameof(rightOperand));
        }

        /// <inheritdoc />
        public override Expression GetExpression()
        {
            return OperatorType.Invoke(LeftOperand, RightOperand);
        }
    }
}
