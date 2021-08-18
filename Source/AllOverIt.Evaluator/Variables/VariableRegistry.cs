using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Helpers;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    public sealed class VariableRegistry : IVariableRegistry
    {
        private readonly IDictionary<string, IVariable> _variableRegistry = new Dictionary<string, IVariable>();
        public IEnumerable<KeyValuePair<string, IVariable>> Variables => _variableRegistry;

        public void AddVariable(IVariable variable)
        {
            _ = variable.WhenNotNull(nameof(variable));

            if (_variableRegistry.ContainsKey(variable.Name))
            {
                throw new VariableException($"The variable '{variable.Name}' is already registered");
            }

            _variableRegistry[variable.Name] = variable;

            // make the variable aware of the registry it is associated with so referenced variables can be resolved (if required)
            variable.SetVariableRegistry(this);
        }

        public void AddVariables(params IVariable[] variables)
        {
            foreach (var variable in variables)
            {
                AddVariable(variable);
            }
        }

        public double GetValue(string name)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            var variable = GetVariable(name);

            return variable.Value;
        }

        public void SetValue(string name, double value)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            if (GetVariable(name) is not IMutableVariable variable)
            {
                throw new VariableImmutableException($"The variable '{name}' is not mutable");
            }

            variable.SetValue(value);
        }

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