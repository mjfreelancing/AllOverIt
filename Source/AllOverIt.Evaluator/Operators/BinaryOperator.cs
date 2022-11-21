using AllOverIt.Assertion;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that operates on two operands.</summary>
    public abstract class BinaryOperator : Operator<Func<Expression, Expression, Expression>>
    {
        internal readonly Expression _leftOperand;
        internal readonly Expression _rightOperand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorType"></param>
        /// <param name="leftOperand">The left operand (argument) of the operator.</param>
        /// <param name="rightOperand">The right operand (argument) of the operator.</param>
        protected BinaryOperator(Func<Expression, Expression, Expression> operatorType, Expression leftOperand, Expression rightOperand)
            : base(operatorType)
        {
            _leftOperand = leftOperand.WhenNotNull(nameof(leftOperand));
            _rightOperand = rightOperand.WhenNotNull(nameof(rightOperand));
        }

        /// <inheritdoc />
        public override Expression GetExpression()
        {
            return OperatorType.Invoke(_leftOperand, _rightOperand);
        }
    }
}
