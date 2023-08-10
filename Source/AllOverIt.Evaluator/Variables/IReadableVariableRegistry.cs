namespace AllOverIt.Evaluator.Variables
{
    /// <summary>Represents a read-only registry of variables referenced by one or more formula.</summary>
    public interface IReadableVariableRegistry
    {
        /// <summary>Gets the current value of a variable based on its name.</summary>
        /// <param name="name">The name of the variable to be evaluated.</param>
        /// <returns>The current value of a variable based on its name.</returns>
        double GetValue(string name);
    }
}