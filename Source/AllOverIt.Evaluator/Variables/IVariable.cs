using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    // Describes a named variable.
    public interface IVariable
    {
        // Gets the variable's name.
        string Name { get; }

        // Gets a list of variables this variable references.
        IEnumerable<IVariable> ReferencedVariables { get; }

        // Gets the variable's value.
        double Value { get; }
    }
}