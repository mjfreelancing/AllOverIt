using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    /// <summary>Describes a named variable.</summary>
    public interface IVariable
    {
        /// <summary>Gets the variable's name.</summary>
        string Name { get; }

        /// <summary>Gets the variable's value.</summary>
        double Value { get; }

        /// <summary>Gets all variables this variable references. Only applicable to variables constructed from a FormulaCompilerResult.</summary>
        IEnumerable<IVariable> ReferencedVariables { get; }
    }
}