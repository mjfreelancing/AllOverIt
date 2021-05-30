using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Stack;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Helpers;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    // Implements a factory that creates expressions required for compiling a formula.
    public sealed class AoiFormulaExpressionFactory : IAoiFormulaExpressionFactory
    {
        // 'expressionStack' contains the expressions required for processing the provided operation
        public Expression CreateExpression(IAoiArithmeticOperation operation, IAoiStack<Expression> expressionStack)
        {
            _ = operation.WhenNotNull(nameof(operation));
            _ = expressionStack.WhenNotNull(nameof(expressionStack));

            // how many parameters need to be passed to it
            var expressionsRequired = operation.ArgumentCount;

            if (expressionStack.Count < expressionsRequired)
            {
                throw new AoiFormulaException($"Insufficient expressions in the stack. {expressionStack.Count} available, {expressionsRequired} required.");
            }

            var expressions = new Stack<Expression>();

            while (expressions.Count < expressionsRequired)
            {
                // the items need to be organized in the reverse order they are popped
                var expression = expressionStack.Pop();
                expressions.Push(expression);
            }

            return operation.GetExpression(expressions.ToArray());
        }

        public Expression CreateExpression(string variableName, IAoiVariableRegistry variableRegistry)
        {
            _ = variableName.WhenNotNullOrEmpty(nameof(variableName));
            _ = variableRegistry.WhenNotNull(nameof(variableRegistry));

            // create an expression that calls GetValue(), passing the name of the variable
            var registry = Expression.Constant(variableRegistry);

            var getValueMethod = typeof(IAoiReadableVariableRegistry).GetMethod("GetValue", new[] { typeof(string) });
            var variableExpression = new Expression[] { Expression.Constant(variableName) };

            return Expression.Call(registry, getValueMethod!, variableExpression);
        }
    }
}