using AllOverIt.Assertion;
using AllOverIt.Evaluator.Variables;

namespace AllOverIt.Evaluator.Extensions
{
    /// <summary>Provides extension methods for IFormulaCompiler.</summary>
    public static class FormulaCompilerExtensions
    {
        /// <summary>Compiles a formula and returns the invoked result.</summary>
        /// <param name="compiler">The IFormulaCompiler instance used to compile the formula.</param>
        /// <param name="formula">The formula as a string.</param>
        /// <param name="variableRegistry">An optional registry where, if required, contains previously populated variables required for the evaluation of the formula.</param>
        /// <returns>The evaluated result.</returns>
        public static double GetResult(this FormulaCompiler compiler, string formula, IVariableRegistry variableRegistry = null)
        {
            _ = compiler.WhenNotNull(nameof(compiler));
            _ = formula.WhenNotNullOrEmpty(nameof(compiler));

            return compiler
              .Compile(formula, variableRegistry)
              .Resolver
              .Invoke();
        }
    }
}