using AllOverIt.Helpers;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that operates on two operands.
    public abstract class BinaryOperator : Operator<Func<Expression, Expression, Expression>>
    {
        private readonly Expression _leftOperand;
        private readonly Expression _rightOperand;

        protected BinaryOperator(Func<Expression, Expression, Expression> operatorType, Expression leftOperand, Expression rightOperand)
          : base(operatorType)
        {
            _leftOperand = leftOperand.WhenNotNull(nameof(leftOperand));
            _rightOperand = rightOperand.WhenNotNull(nameof(rightOperand));
        }

        // Gets an Expression that is the result of invoking the operator.
        public override Expression GetExpression()
        {
            return OperatorType.Invoke(_leftOperand, _rightOperand);
        }
    }
}
