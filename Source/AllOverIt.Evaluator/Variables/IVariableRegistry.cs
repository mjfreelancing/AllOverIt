namespace AllOverIt.Evaluator.Variables
{
    public interface IVariableRegistry : IReadableVariableRegistry
    {
        // Adds a new variable to the registry.
        void AddVariable(IVariable variable);

        // Adds one or more variables to the registry.
        void AddVariables(params IVariable[] variables);

        // Sets the value of a variable based on its name.
        void SetValue(string name, double value);
    }
}