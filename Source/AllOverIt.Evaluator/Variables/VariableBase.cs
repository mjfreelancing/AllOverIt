using AllOverIt.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables
{
    // An abstract base class for a named variable.
    public abstract record VariableBase : IVariable
    {
        internal IVariableRegistry VariableRegistry { get; set; }

        public string Name { get; }

        /// <summary>Gets the variable's value.</summary>
        public abstract double Value { get; }

        /// <summary>Gets all variables this variable references. Only applicable to variables constructed from a FormulaCompilerResult.</summary>
        public IEnumerable<IVariable> ReferencedVariables { get; protected init; } = Enumerable.Empty<IVariable>();

        // 'referencedVariableNames' is an optional list of variable names that this variable depends on to calculate its value.
        protected VariableBase(string name)
        {
            Name = name.WhenNotNullOrEmpty(nameof(name));
        }
    }
}