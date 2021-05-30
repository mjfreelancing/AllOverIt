using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    // Describes a mechanism for looking up referenced and referencing variables.
    public interface IAoiVariableLookup
    {
        // Get all variables referenced by the specified variable using the specified lookupMode.
        IEnumerable<IAoiVariable> GetReferencedVariables(IAoiVariable variable, AoiVariableLookupMode lookupMode);

        // Get all variables referencing the specified variable using the specified lookupMode.
        IEnumerable<IAoiVariable> GetReferencingVariables(IAoiVariable variable, AoiVariableLookupMode lookupMode);
    }
}