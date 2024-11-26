namespace AllOverIt.Evaluator.Variables
{
    /// <summary>Represents a registry of variables that may be explicitly evaluated, or implicitly referenced by one or more formula.</summary>
    public interface IVariableRegistry : IReadableVariableRegistry, IEnumerable<KeyValuePair<string, IVariable>>
    {
        /// <summary>Adds a new variable to the registry.</summary>
        /// <param name="variable">The variable to add to the registry.</param>
        void AddVariable(IVariable variable);

        /// <summary>Adds one or more variables to the registry.</summary>
        /// <param name="variables">The variables to add to the registry.</param>
        void AddVariables(params IVariable[] variables);

        /// <summary>Sets the value of a variable based on its name.</summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        void SetValue(string name, double value);

        /// <summary>Clears all variables from the registry.</summary>
        void Clear();

        /// <summary>Tries to get a variable, by name, from the variable registry.</summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="variable">The variable to try to get from the variable registry.</param>
        /// <returns><see langword="True"/> if the variable was retrieved, otherwise <see langword="False"/>.</returns>
        bool TryGetVariable(string name, out IVariable? variable);

        /// <summary>Gets a variable from the variable registry, by name.</summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The requested variable.</returns>
        IVariable GetVariable(string name);

        /// <summary>Determines if a variable is registered with the registry.</summary>
        /// <param name="name">The name of the variable to look up.</param>
        /// <returns><see langword="True"/> if the variable is registered, otherwise <see langword="False"/>.</returns>
        bool ContainsVariable(string name);
    }
}