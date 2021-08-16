using AllOverIt.Evaluator.Operators;
using AllOverIt.Helpers;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An abstract base class for all arithmetic operators and user defined methods.
    public abstract class ArithmeticOperationBase : IArithmeticOperation
    {
        // A delegate that creates an instance of a concrete IOperator. The Expression array must have as many elements as is specified by the ArgumentCount property.
        private readonly Func<Expression[], IOperator> _creator;

        // The number arguments required by the operator or user defined method.
        public int ArgumentCount { get; }

        protected ArithmeticOperationBase(int argumentCount, Func<Expression[], IOperator> creator)
        {
            ArgumentCount = argumentCount;
            _creator = creator.WhenNotNull(nameof(creator));
        }

        // Uses the delegate passed to the constructor to create an instance of the required operator and returns its associated expression.
        // 'expressions' is the array of arguments required to initialize the operator or user defined method.
        public Expression GetExpression(Expression[] expressions)
        {
            // Note: Cannot cache the result of Creator.Invoke(expressions) because the resultant expression is dependent on the input expressions.
            return _creator.Invoke(expressions).GetExpression();
        }
    }
}