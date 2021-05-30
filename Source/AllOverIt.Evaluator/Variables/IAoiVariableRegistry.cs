namespace AllOverIt.Evaluator.Variables
{
    public interface IAoiVariableRegistry : IAoiReadableVariableRegistry
    {
        // Adds a new variable to the registry.
        void AddVariable(IAoiVariable variable);

        // Adds one or more variables to the registry.
        void AddVariables(params IAoiVariable[] variables);

        // Sets the value of a variable based on its name.
        void SetValue(string name, double value);
    }
}