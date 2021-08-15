using AllOverIt.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables.Extensions
{
    public static class FormulaCompilerResultExtensions
    {
        public static IEnumerable<IVariable> GetReferencedVariables(this FormulaCompilerResult compilerResult)
        {
            var registry = compilerResult.VariableRegistry;
            var referencedNames = compilerResult.ReferencedVariableNames;

            return referencedNames.SelectAsReadOnlyCollection(variableName =>
                registry.Variables.Single(item => item.Key == variableName).Value);
        }
    }
}