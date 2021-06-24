namespace AllOverIt.Evaluator.Variables
{
    // Provides the ability to set a variable's value.
    public interface IMutableVariable : IVariable
    {
        // Sets a new value on the variable.
        void SetValue(double value);
    }
}