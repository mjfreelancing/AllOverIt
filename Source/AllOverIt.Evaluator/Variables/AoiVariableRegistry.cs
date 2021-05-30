using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Helpers;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    internal sealed class AoiVariableRegistry : IAoiVariableRegistry
    {
        private readonly IDictionary<string, IAoiVariable> _variableRegistry;
        public IEnumerable<KeyValuePair<string, IAoiVariable>> Variables => _variableRegistry;

        public AoiVariableRegistry()
            : this(new Dictionary<string, IAoiVariable>())
        {
        }

        internal AoiVariableRegistry(IDictionary<string, IAoiVariable> variableRegistry)
        {
            _variableRegistry = variableRegistry.WhenNotNull(nameof(variableRegistry));
        }

        public void AddVariable(IAoiVariable variable)
        {
            _ = variable.WhenNotNull(nameof(variable));

            if (_variableRegistry.ContainsKey(variable.Name))
            {
                throw new AoiVariableException($"The variable '{variable.Name}' is already registered");
            }

            _variableRegistry[variable.Name] = variable;

            // make the variable aware of the registry it is associated with so referenced variables can be resolved (if required)
            variable.SetVariableRegistry(this);
        }

        public void AddVariables(params IAoiVariable[] variables)
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

            if (GetVariable(name) is not IAoiMutableVariable variable)
            {
                throw new AoiVariableNotMutableException($"The variable '{name}' is not mutable");
            }

            variable.SetValue(value);
        }

        private IAoiVariable GetVariable(string name)
        {
            if (!_variableRegistry.TryGetValue(name, out var variable))
            {
                throw new AoiVariableException($"The variable '{name}' is not registered");
            }

            return variable;
        }
    }
}