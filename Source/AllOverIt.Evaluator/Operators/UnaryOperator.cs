using AllOverIt.Helpers;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An expression operator that operates on a single operand to create a new expression as the result.
    public abstract class UnaryOperator : Operator<Func<Expression, Expression>>
    {
        internal Expression Operand { get; set; }

        protected UnaryOperator(Func<Expression, Expression> operatorType, Expression operand)
          : base(operatorType)
        {
            Operand = operand.WhenNotNull(nameof(operand));
        }

        // Gets an Expression that is the result of invoking the operator.
        public override Expression GetExpression()
        {
            return OperatorType.Invoke(Operand);
        }
    }
}
