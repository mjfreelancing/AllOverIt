using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables
{
    // An abstract base class for a named variable.
    public abstract class AoiVariableBase : IAoiVariable
    {
        private readonly Lazy<IEnumerable<IAoiVariable>> _referencedVariables;
        internal IAoiVariableRegistry VariableRegistry { get; set; }

        public string Name { get; }

        // Gets a list of variables this variable references.
        // If the variable instance is not initialized then an empty list is returned.
        public IEnumerable<IAoiVariable> ReferencedVariables => _referencedVariables.Value;

        /// <summary>Gets the variable's value.</summary>
        public abstract double Value { get; }

        // 'referencedVariableNames' is an optional list of variable names that this variable depends on to calculate its value.
        protected AoiVariableBase(string name, IEnumerable<string> referencedVariableNames)
        {
            Name = name.WhenNotNullOrEmpty(nameof(name));

            var referencedNames = referencedVariableNames != null
              ? referencedVariableNames.AsReadOnlyList()
              : Enumerable.Empty<string>();

            _referencedVariables = new Lazy<IEnumerable<IAoiVariable>>(() => GetReferencedVariables(referencedNames));
        }

        private IEnumerable<IAoiVariable> GetReferencedVariables(IEnumerable<string> referencedVariableNames)
        {
            _ = VariableRegistry.WhenNotNull(nameof(VariableRegistry));

            return (from keyValue in VariableRegistry.Variables
                    where referencedVariableNames.Contains(keyValue.Key)
                    select keyValue.Value).AsReadOnlyList();
        }
    }
}