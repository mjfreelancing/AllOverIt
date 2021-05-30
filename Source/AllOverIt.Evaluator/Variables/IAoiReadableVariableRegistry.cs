using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    // Describes a read-only registry of variables.
    public interface IAoiReadableVariableRegistry
    {
        // Gets an enumerable of all variables and their associated name contained in the registry.
        IEnumerable<KeyValuePair<string, IAoiVariable>> Variables { get; }

        // Gets the current value of a variable based on its name.
        double GetValue(string name);
    }
}