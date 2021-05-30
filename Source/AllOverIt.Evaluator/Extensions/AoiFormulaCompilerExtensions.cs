using AllOverIt.Evaluator.Variables;
using AllOverIt.Helpers;

namespace AllOverIt.Evaluator.Extensions
{
    public static class AoiFormulaCompilerExtensions
    {
        // Compiles a formula and returns the invoked result
        public static double GetResult(this IAoiFormulaCompiler compiler, string formula, IAoiVariableRegistry variableRegistry = null)
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