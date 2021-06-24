using AllOverIt.Evaluator.Variables;

namespace AllOverIt.Evaluator
{
    // An interface used to parse a formula.
    public interface IFormulaParser
    {
        // Parses a formula to an FormulaProcessorResult instance that contains a list of referenced variables and a delegate that can 
        // be later compiled and invoked.
        FormulaProcessorResult Parse(string formula, IVariableRegistry variableRegistry);
    }
}