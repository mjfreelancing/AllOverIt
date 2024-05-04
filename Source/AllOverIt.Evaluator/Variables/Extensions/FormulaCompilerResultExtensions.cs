using AllOverIt.Extensions;

namespace AllOverIt.Evaluator.Variables.Extensions
{
    /// <summary>Provides a variety of <see cref="FormulaCompilerResult"/> extensions.</summary>
    public static class FormulaCompilerResultExtensions
    {
        /// <summary>Gets all referenced variables for a compiled formula.</summary>
        /// <param name="formulaCompilerResult">The compiled formula result.</param>
        /// <returns>All referenced variables for a compiled formula.</returns>
        public static IEnumerable<IVariable> GetReferencedVariables(this FormulaCompilerResult formulaCompilerResult)
        {
            var registry = formulaCompilerResult.VariableRegistry;
            var referencedNames = formulaCompilerResult.ReferencedVariableNames;

            return referencedNames.SelectAsReadOnlyCollection(variableName =>
                registry.Single(item => item.Key == variableName).Value);
        }
    }
}