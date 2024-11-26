using AllOverIt.Assertion;

namespace AllOverIt.Evaluator.Variables
{
    /// <summary>An abstract base class for a named variable.</summary>
    public abstract class VariableBase : IVariable
    {
        internal IVariableRegistry? VariableRegistry { get; set; }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>Gets the variable's value.</summary>
        public abstract double Value { get; }

        /// <summary>Gets all variables this variable references. Only applicable to variables constructed from a FormulaCompilerResult.</summary>
        public IVariable[] ReferencedVariables { get; protected init; } = [];

        /// <summary>Constructor.</summary>
        /// <param name="name">The name of the variable.</param>
        protected VariableBase(string name)
        {
            Name = name.WhenNotNullOrEmpty();
        }
    }
}