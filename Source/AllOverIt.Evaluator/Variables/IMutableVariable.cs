namespace AllOverIt.Evaluator.Variables
{
    /// <summary>A variable that can have its value changed.</summary>
    public interface IMutableVariable : IVariable
    {
        /// <summary>Sets a new value on the variable.</summary>
        /// <param name="value">The new value to be assigned.</param>
        void SetValue(double value);
    }
}