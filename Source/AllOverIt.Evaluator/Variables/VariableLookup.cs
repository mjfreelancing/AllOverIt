using AllOverIt.Assertion;
using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables
{
    /// <summary>Provides the ability to lookup referenced and referencing variables from a variable registry.</summary>
    public sealed class VariableLookup : IVariableLookup
    {
        private readonly IVariableRegistry _variableRegistry;

        /// <summary>Constructor.</summary>
        /// <param name="variableRegistry">The variable registry used for lookup operations.</param>
        public VariableLookup(IVariableRegistry variableRegistry)
        {
            _variableRegistry = variableRegistry.WhenNotNull(nameof(variableRegistry));
        }

        /// <inheritdoc />
        public IEnumerable<IVariable> GetReferencedVariables(IVariable variable, VariableLookupMode lookupMode)
        {
            return lookupMode == VariableLookupMode.Explicit
              ? variable.ReferencedVariables
              : variable.GetAllReferencedVariables();
        }

        /// <inheritdoc />
        public IEnumerable<IVariable> GetReferencingVariables(IVariable variable, VariableLookupMode lookupMode)
        {
            var variables = new List<IVariable>();

            foreach (var keyValue in _variableRegistry.Variables)
            {
                var registryVariable = keyValue.Value;
                var referencedVariables = GetReferencedVariables(registryVariable, lookupMode);

                foreach (var referenced in referencedVariables)
                {
                    if (ReferenceEquals(variable, referenced))
                    {
                        variables.Add(registryVariable);
                    }
                }
            }

            return variables;

            // LINQ version is less performing

            //return (from keyValue in _variableRegistry.Variables
            //        let registryVariable = keyValue.Value
            //        let referencedVariables = GetReferencedVariables(registryVariable, lookupMode)
            //        from referenced in referencedVariables
            //        where ReferenceEquals(variable, referenced)
            //        select registryVariable)
            //  .AsReadOnlyList();
        }
    }
}