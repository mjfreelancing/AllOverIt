using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    // Describes a named variable.
    public interface IAoiVariable
    {
        // Gets the variable's name.
        string Name { get; }

        // Gets a list of variables this variable references.
        IEnumerable<IAoiVariable> ReferencedVariables { get; }

        // Gets the variable's value.
        double Value { get; }
    }
}