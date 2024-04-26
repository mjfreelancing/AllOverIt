using AllOverIt.Assertion;
using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An abstract base class for all arithmetic operators and user defined methods.</summary>
    public abstract class ArithmeticOperationBase : IArithmeticOperation
    {
        // A delegate that creates an instance of a concrete IOperator. The Expression array must have as many elements as is specified by the ArgumentCount property.
        internal readonly Func<Expression[], IOperator> _creator;

        /// <inheritdoc />
        public int ArgumentCount { get; }

        /// <summary>Constructor.</summary>
        /// <param name="argumentCount">The number of required arguments.</param>
        /// <param name="creator">The factory that creates the required operation using the provided arguments (as expressions).</param>
        protected ArithmeticOperationBase(int argumentCount, Func<Expression[], IOperator> creator)
        {
            ArgumentCount = argumentCount;
            _creator = creator.WhenNotNull(nameof(creator));
        }

        /// <inheritdoc />
        public Expression GetExpression(Expression[] expressions)
        {
            // Uses the delegate passed to the constructor to create an instance of the required operator and returns its associated expression.
            // 'expressions' is the array of arguments required to initialize the operator or user defined method.

            // Note: Cannot cache the result of Creator.Invoke(expressions) because the resultant expression is dependent on the input expressions.
            return _creator.Invoke(expressions).GetExpression();
        }
    }
}