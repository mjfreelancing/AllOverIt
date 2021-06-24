using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    // Describes a mechanism for looking up referenced and referencing variables.
    public interface IVariableLookup
    {
        // Get all variables referenced by the specified variable using the specified lookupMode.
        IEnumerable<IVariable> GetReferencedVariables(IVariable variable, VariableLookupMode lookupMode);

        // Get all variables referencing the specified variable using the specified lookupMode.
        IEnumerable<IVariable> GetReferencingVariables(IVariable variable, VariableLookupMode lookupMode);
    }
}