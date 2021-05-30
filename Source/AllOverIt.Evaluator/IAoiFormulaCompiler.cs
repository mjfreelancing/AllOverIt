using AllOverIt.Evaluator.Variables;

namespace AllOverIt.Evaluator
{
    // An interface representing a formula compiler that compiles a string formula to a delegate that can be invoked.
    public interface IAoiFormulaCompiler
    {
        // Compiles a formula to a compiled delegate.
        // 'variableRegistry' is a registry of variables referenced by the formula.
        AoiFormulaCompilerResult Compile(string formula, IAoiVariableRegistry variableRegistry);
    }
}