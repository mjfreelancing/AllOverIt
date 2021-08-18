using AllOverIt.Evaluator.Variables;

namespace AllOverIt.Evaluator
{
    /// <summary>Compiles a mathematical expression provided as a string to a delegate that can be repeatedly invoked for re-evaluation.</summary>
    public interface IFormulaCompiler
    {
        /// <summary>Compiles a formula represented as a string into a delegate that can be repeatedly invoked for re-evaluation.</summary>
        /// <param name="formula">The mathematical formula to be compiled.</param>
        /// <param name="variableRegistry">The registry of variables referenced by the formula that are required for evaluation. This
        /// can be populated with all required variables before or after compilation, but before evaluation. If a registry is not provided
        /// then an instance will be created. In both cases, the registry will be included with the compiler result.</param>
        /// <returns>The compiler result containing the variable registry, the compiled delegate, and a list of variables referenced by the
        /// formula (if applicable).</returns>
        FormulaCompilerResult Compile(string formula, IVariableRegistry variableRegistry = null);
    }
}