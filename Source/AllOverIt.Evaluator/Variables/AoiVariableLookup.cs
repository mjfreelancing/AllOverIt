using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables
{
    // Provides the ability to lookup referenced and referencing variables from a variable registry.
    public sealed class AoiVariableLookup : IAoiVariableLookup
    {
        internal IAoiVariableRegistry VariableRegistry { get; }

        public AoiVariableLookup(IAoiVariableRegistry variableRegistry)
        {
            VariableRegistry = variableRegistry.WhenNotNull(nameof(variableRegistry));
        }

        public IEnumerable<IAoiVariable> GetReferencedVariables(IAoiVariable variable, AoiVariableLookupMode lookupMode)
        {
            return lookupMode == AoiVariableLookupMode.Explicit
              ? variable.ReferencedVariables
              : variable.GetAllReferencedVariables();
        }

        public IEnumerable<IAoiVariable> GetReferencingVariables(IAoiVariable variable, AoiVariableLookupMode lookupMode)
        {
            return (from keyValue in VariableRegistry.Variables
                    let registryVariable = keyValue.Value
                    let referencedVariables = GetReferencedVariables(registryVariable, lookupMode)
                    from referenced in referencedVariables
                    where ReferenceEquals(variable, referenced)
                    select registryVariable)
              .AsReadOnlyList();
        }
    }
}