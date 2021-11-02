using AllOverIt.Assertion;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables
{
    /// <summary>An abstract base class for a named variable.</summary>
    public abstract record VariableBase : IVariable
    {
        internal IVariableRegistry VariableRegistry { get; set; }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>Gets the variable's value.</summary>
        public abstract double Value { get; }

        /// <summary>Gets all variables this variable references. Only applicable to variables constructed from a FormulaCompilerResult.</summary>
        public IEnumerable<IVariable> ReferencedVariables { get; protected init; } = Enumerable.Empty<IVariable>();

        /// <summary>Constructor.</summary>
        /// <param name="name">The name of the ariable.</param>
        protected VariableBase(string name)
        {
            Name = name.WhenNotNullOrEmpty(nameof(name));
        }
    }
}