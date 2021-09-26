using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    /// <summary>A read-only registry of variables.</summary>
    public interface IReadableVariableRegistry
    {
        /// <summary>Gets an enumerable of all variables and their associated name contained in the registry.</summary>
        IEnumerable<KeyValuePair<string, IVariable>> Variables { get; }

        /// <summary>Gets the current value of a variable based on its name.</summary>
        /// <param name="name">The name of the variable to be evaluated.</param>
        /// <returns>The current value of a variable based on its name.</returns>
        double GetValue(string name);
    }
}