using AllOverIt.Assertion;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that operates on three operands.</summary>
    public abstract class TernaryOperator : Operator<Func<Expression, Expression, Expression, Expression>>
    {
        internal readonly Expression _operand1;
        internal readonly Expression _operand2;
        internal readonly Expression _operand3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorType"></param>
        /// <param name="operand1">The first operand (argument) of the operator.</param>
        /// <param name="operand2">The second operand (argument) of the operator.</param>
        /// <param name="operand3">The third operand (argument) of the operator.</param>
        protected TernaryOperator(Func<Expression, Expression, Expression, Expression> operatorType, Expression operand1, Expression operand2, Expression operand3)
            : base(operatorType)
        {
            _operand1 = operand1.WhenNotNull(nameof(operand1));
            _operand2 = operand2.WhenNotNull(nameof(operand2));
            _operand3 = operand3.WhenNotNull(nameof(operand3));
        }

        /// <inheritdoc />
        public override Expression GetExpression()
        {
            return OperatorType.Invoke(_operand1, _operand2, _operand3);
        }
    }
}