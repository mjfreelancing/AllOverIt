using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables
{
    // An abstract base class for a named variable.
    public abstract class VariableBase : IVariable
    {
        private readonly Lazy<IEnumerable<IVariable>> _referencedVariables;
        internal IVariableRegistry VariableRegistry { get; set; }

        public string Name { get; }

        // Gets a list of variables this variable references.
        // If the variable instance is not initialized then an empty list is returned.
        public IEnumerable<IVariable> ReferencedVariables => _referencedVariables.Value;

        /// <summary>Gets the variable's value.</summary>
        public abstract double Value { get; }

        // 'referencedVariableNames' is an optional list of variable names that this variable depends on to calculate its value.
        protected VariableBase(string name, IEnumerable<string> referencedVariableNames)
        {
            Name = name.WhenNotNullOrEmpty(nameof(name));

            var referencedNames = referencedVariableNames != null
              ? referencedVariableNames.AsReadOnlyList()
              : Enumerable.Empty<string>();

            _referencedVariables = new Lazy<IEnumerable<IVariable>>(() => GetReferencedVariables(referencedNames));
        }

        private IEnumerable<IVariable> GetReferencedVariables(IEnumerable<string> referencedVariableNames)
        {
            _ = VariableRegistry.WhenNotNull(nameof(VariableRegistry));

            return (from keyValue in VariableRegistry.Variables
                    where referencedVariableNames.Contains(keyValue.Key)
                    select keyValue.Value).AsReadOnlyList();
        }
    }
}