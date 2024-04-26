namespace AllOverIt.Evaluator.Variables
{
    /// <summary>Describes a named variable.</summary>
    public interface IVariable
    {
        /// <summary>Gets the name of the variable.</summary>
        string Name { get; }

        /// <summary>Gets the value of the variable.</summary>
        double Value { get; }

        /// <summary>Gets all variables this variable references. Only applicable to variables constructed from a FormulaCompilerResult.</summary>
        IEnumerable<IVariable> ReferencedVariables { get; }
    }
}