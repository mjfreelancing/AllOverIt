using AllOverIt.Evaluator.Variables;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    // An interface representing a processor that converts a formula to an expression that can later be invoked to obtain its result.
    public interface IAoiFormulaProcessor
    {
        // Processes a formula to create a delegate that can later be invoked to obtain its result.
        AoiFormulaProcessorResult Process(IAoiFormulaReader formulaReader, IAoiVariableRegistry variableRegistry);

        // Pushes an operator token into a stack that will be later used for creating an associated expression.
        void PushOperator(string operatorToken);

        // Pushes an expression into a stack that will later be used for the input of another expression as the formula's operations are processed.
        void PushExpression(Expression expression);

        // Processes the start of a new expression scope (such as grouped mathematical operations or method arguments).
        void ProcessScopeStart();

        // Processes the end of an expression scope (such as grouped mathematical operations or method arguments).
        bool ProcessScopeEnd(bool isUserMethod);

        // Processes any queued operators associated with a parsed method argument.
        void ProcessMethodArgument();

        // Processes an operator at the current position of the formula reader.
        void ProcessOperator();

        // Processes a numerical value at the current position of the formula reader.
        void ProcessNumerical();

        // Processes a named operand (variable or user method name) at the current position of the formula reader.
        void ProcessNamedOperand();
    }
}