using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    // An abstract class for defining an operator that can create an expression for the operation it performs.
    public abstract class OperatorBase : IOperator
    {
        // Gets an <c>Expression</c> that is the result of invoking the operator.
        public abstract Expression GetExpression();

        // Creates a concrete instance of an operator. 
        // 'TOperator' is the resultant concrete operator type. It must inherit from IAoiOperator and the constructor must contain the required number of arguments.
        // 'expressions' are the arguments to be passed to the constructor of the operator being created.
        // Throws AoiOperatorException when the number of arguments passed in 'expressions' does not match the number of arguments expected by the operator's constructor.
        // Throws 'InvalidOperationException' when the operator contains more than one constructor.
        public static TOperator Create<TOperator>(Expression[] expressions, Func<Expression[], TOperator> creator)
          where TOperator : IOperator
        {
            _ = expressions.WhenNotNullOrEmpty(nameof(expressions));
            _ = creator.WhenNotNull(nameof(creator));

            var constructorArgumentCount = typeof(TOperator).GetConstructors().Single().GetParameters().Length;

            if (expressions.Length != constructorArgumentCount)
            {
                throw new OperatorException($"Invalid number of arguments. {constructorArgumentCount} expressions expected, {expressions.Length} provided.");
            }

            return creator.Invoke(expressions);
        }
    }
}