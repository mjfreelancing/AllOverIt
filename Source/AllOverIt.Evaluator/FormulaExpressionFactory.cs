using AllOverIt.Assertion;
using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Reflection;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Evaluator
{
    /// <summary>Implements a factory that creates expressions required for compiling a formula.</summary>
    public static class FormulaExpressionFactory
    {
        private static readonly MethodInfo GetValueMethodInfo = typeof(IReadableVariableRegistry).GetMethod("GetValue", [CommonTypes.StringType]);

        /// <summary>Creates an Expression for an arithmetic operation.</summary>
        /// <param name="operation">The arithmetic operation to create an Expression for.</param>
        /// <param name="expressionStack">Contains the expressions required for processing the required operation.</param>
        /// <returns>An Expression representing the arithmetic operation.</returns>
        public static Expression CreateExpression(IArithmeticOperation operation, Stack<Expression> expressionStack)
        {
            _ = operation.WhenNotNull(nameof(operation));
            _ = expressionStack.WhenNotNull(nameof(expressionStack));

            // how many parameters need to be passed to it
            var expressionsRequired = operation.ArgumentCount;

            if (expressionStack.Count < expressionsRequired)
            {
                throw new FormulaException($"Insufficient expressions in the stack. {expressionStack.Count} available, {expressionsRequired} required.");
            }

            var expressions = new Stack<Expression>();

            while (expressions.Count < expressionsRequired)
            {
                // the items need to be organized in the reverse order they are popped
                var expression = expressionStack.Pop();
                expressions.Push(expression);
            }

            return operation.GetExpression([.. expressions]);
        }

        /// <summary>Creates an expression that gets the value of a variable from the variable registry.</summary>
        /// <param name="variableName">The name of the variable to get the value for.</param>
        /// <param name="variableRegistry">The variable registry containing the required value.</param>
        /// <returns>An expression that gets the value of a variable from the variable registry.</returns>
        public static Expression CreateExpression(string variableName, IVariableRegistry variableRegistry)
        {
            _ = variableName.WhenNotNullOrEmpty(nameof(variableName));
            _ = variableRegistry.WhenNotNull(nameof(variableRegistry));

            // create an expression that calls GetValue(), passing the name of the variable
            var registry = Expression.Constant(variableRegistry);
            return Expression.Call(registry, GetValueMethodInfo, Expression.Constant(variableName));
        }
    }
}