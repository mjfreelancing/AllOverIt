using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables.Extensions
{
    public static class AoiVariableExtensions
    {
        // Associates a variable with the specified variable registry if it inherits from AoiVariableBase.
        public static void SetVariableRegistry(this IAoiVariable variable, IAoiVariableRegistry variableRegistry)
        {
            _ = variable.WhenNotNull(nameof(variable));
            _ = variableRegistry.WhenNotNull(nameof(variableRegistry));

            if (variable is AoiVariableBase variableBase)
            {
                variableBase.VariableRegistry = variableRegistry;
            }
        }

        // Gets a read-only list of all variables referenced by the variable (explicit and implicit).
        public static IReadOnlyList<IAoiVariable> GetAllReferencedVariables(this IAoiVariable variable)
        {
            _ = variable.WhenNotNull(nameof(variable));

            var allVariables = new List<IAoiVariable>();

            GetAllReferencedVariables(variable.ReferencedVariables, allVariables);

            return allVariables
              .Distinct()
              .AsReadOnlyList();
        }

        private static void GetAllReferencedVariables(IEnumerable<IAoiVariable> referencedVariables, List<IAoiVariable> allVariables)
        {
            var variables = referencedVariables.AsReadOnlyList();

            allVariables.AddRange(variables);

            foreach (var variable in variables)
            {
                GetAllReferencedVariables(variable.ReferencedVariables, allVariables);
            }
        }
    }
}