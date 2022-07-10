using AllOverIt.Assertion;
using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Variables.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    /// <summary>A registry of variables referenced by one or more formula.</summary>
    public sealed class VariableRegistry : IVariableRegistry
    {
        private readonly IDictionary<string, IVariable> _variableRegistry = new Dictionary<string, IVariable>();

        /// <summary>Contains a key-value collection of all registered variables, keyed by their name.</summary>
        public IEnumerable<KeyValuePair<string, IVariable>> Variables => _variableRegistry;

        /// <inheritdoc />
        public void AddVariable(IVariable variable)
        {
            _ = variable.WhenNotNull(nameof(variable));

            if (_variableRegistry.ContainsKey(variable.Name))
            {
                throw new VariableException($"The variable '{variable.Name}' is already registered.");
            }

            _variableRegistry[variable.Name] = variable;

            // make the variable aware of the registry it is associated with so referenced variables can be resolved (if required)
            variable.SetVariableRegistry(this);
        }

        /// <inheritdoc />
        public void AddVariables(params IVariable[] variables)
        {
            foreach (var variable in variables)
            {
                AddVariable(variable);
            }
        }

        /// <inheritdoc />
        public double GetValue(string name)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            var variable = GetVariable(name);

            return variable.Value;
        }

        /// <inheritdoc />
        public void SetValue(string name, double value)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            if (GetVariable(name) is not IMutableVariable variable)
            {
                throw new VariableImmutableException($"The variable '{name}' is not mutable");
            }

            variable.SetValue(value);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _variableRegistry.Clear();
        }

        private IVariable GetVariable(string name)
        {
            if (!_variableRegistry.TryGetValue(name, out var variable))
            {
                throw new VariableException($"The variable '{name}' is not registered");
            }

            return variable;
        }
    }
}