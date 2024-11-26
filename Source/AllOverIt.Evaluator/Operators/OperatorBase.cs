using AllOverIt.Assertion;
using AllOverIt.Evaluator.Exceptions;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An abstract class for defining an operator that can create an expression for the operation it performs.</summary>
    public abstract class OperatorBase : IOperator
    {
        /// <inheritdoc />
        public abstract Expression GetExpression();

        /// <summary>A factory method to create a concrete instance of an operator.</summary>
        /// <typeparam name="TOperator">The type implementing IOperator with a constructor containing the required number of arguments.</typeparam>
        /// <param name="expressions">The arguments (as Expressions) to be passed to the constructor of the operator being created.</param>
        /// <param name="creator">The factory method used to build the operator based on the provided arguments.</param>
        /// <returns>The concrete IOperator instance.</returns>
        /// <exception cref="OperatorException">When the number of arguments passed in 'expressions' does not match the number of arguments expected by the operator's constructor.</exception>
        /// <exception cref="InvalidOperationException">When the operator contains more than one constructor.</exception>
        public static TOperator Create<TOperator>(Expression[] expressions, Func<Expression[], TOperator> creator)
          where TOperator : IOperator
        {
            _ = expressions.WhenNotNullOrEmpty();
            _ = creator.WhenNotNull();

            var constructorArgumentCount = typeof(TOperator).GetConstructors().Single().GetParameters().Length;

            if (expressions.Length != constructorArgumentCount)
            {
                throw new OperatorException($"Invalid number of arguments. {constructorArgumentCount} expressions expected, {expressions.Length} provided.");
            }

            return creator.Invoke(expressions);
        }
    }
}