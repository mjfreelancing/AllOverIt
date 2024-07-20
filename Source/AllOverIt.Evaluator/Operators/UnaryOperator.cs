using AllOverIt.Assertion;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An expression operator that operates on a single operand to create a new expression as the result.</summary>
    public abstract class UnaryOperator : Operator<Func<Expression, Expression>>
    {
        internal readonly Expression _operand;

        /// <summary>Constructor.</summary>
        /// <param name="operatorType">The unary operator that returns the result of an operand (as Expressions).</param>
        /// <param name="operand">The operand (argument) to be evaluated.</param>
        protected UnaryOperator(Func<Expression, Expression> operatorType, Expression operand)
            : base(operatorType)
        {
            _operand = operand.WhenNotNull();
        }

        /// <inheritdoc />
        public override Expression GetExpression()
        {
            return OperatorType.Invoke(_operand);
        }
    }
}
