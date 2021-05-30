using AllOverIt.Evaluator.Variables;

namespace AllOverIt.Evaluator
{
    // An interface used to parse a formula.
    public interface IAoiFormulaParser
    {
        // Parses a formula to an AoiFormulaProcessorResult instance that contains a list of referenced variables and a delegate that can 
        // be later compiled and invoked.
        AoiFormulaProcessorResult Parse(string formula, IAoiVariableRegistry variableRegistry);
    }
}