using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Stack;
using AllOverIt.Evaluator.Variables;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    // An interface representing a factory that creates expressions required for compiling a formula.
    public interface IAoiFormulaExpressionFactory
    {
        // Creates an Expression for a given operation.
        Expression CreateExpression(IAoiArithmeticOperation operation, IAoiStack<Expression> expressionStack);

        // Creates an expression for obtaining the value of a given variable.
        Expression CreateExpression(string variableName, IAoiVariableRegistry variableRegistry);
    }
}