using AllOverIt.Helpers;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that operates on two operands.
    public abstract class AoiBinaryOperator : AoiOperator<Func<Expression, Expression, Expression>>
    {
        internal Expression LeftOperand { get; }
        internal Expression RightOperand { get; }

        protected AoiBinaryOperator(Func<Expression, Expression, Expression> operatorType, Expression leftOperand, Expression rightOperand)
          : base(operatorType)
        {
            LeftOperand = leftOperand.WhenNotNull(nameof(leftOperand));
            RightOperand = rightOperand.WhenNotNull(nameof(rightOperand));
        }

        // Gets an Expression that is the result of invoking the operator.
        public override Expression GetExpression()
        {
            return OperatorType.Invoke(LeftOperand, RightOperand);
        }
    }
}
