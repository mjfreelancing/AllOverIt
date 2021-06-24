using AllOverIt.Evaluator.Variables;
using AllOverIt.Helpers;

namespace AllOverIt.Evaluator.Extensions
{
    public static class FormulaCompilerExtensions
    {
        // Compiles a formula and returns the invoked result
        public static double GetResult(this IFormulaCompiler compiler, string formula, IVariableRegistry variableRegistry = null)
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