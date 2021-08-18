namespace AllOverIt.Evaluator.Variables
{
    public interface IVariableRegistry : IReadableVariableRegistry
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
    }
}