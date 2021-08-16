using AllOverIt.Evaluator.Variables;

namespace AllOverIt.Evaluator
{
    // An interface representing a formula compiler that compiles a string formula to a delegate that can be invoked.
    public interface IFormulaCompiler
    {
        // Compiles a formula to a compiled delegate.
        // A new VariableRegistry (a registry of variables referenced by the formula) will be created if variableRegistry is null,
        // and it will be returned as part of the FormulaCompilerResult. The variable registry does not have to be populated at the
        // time of compilation.
        
        /// <summary>Compiles a formula represented as a string into a delegate that can be repeatedly invoked for re-evaluation.</summary>
        /// <param name="formula">The mathematical formula to be compiled.</param>
        /// <param name="variableRegistry">The registry of variables referenced by the formula that are required for evaluation. This
        /// can be populated with all required variables before or after compilation, but before evaluation.</param>
        /// <returns></returns>
        FormulaCompilerResult Compile(string formula, IVariableRegistry variableRegistry = null);
    }
}