using AllOverIt.Helpers;

namespace AllOverIt.Evaluator.Variables.Extensions
{
    public static class AoiVariableRegistryExtensions
    {
        // Adds a variable to a variable registry and returns the registry to provide a fluent syntax.
        public static IAoiVariableRegistry Add(this IAoiVariableRegistry registry, IAoiVariable variable)
        {
            _ = registry.WhenNotNull(nameof(registry));
            _ = variable.WhenNotNull(nameof(variable));

            registry.AddVariable(variable);

            return registry;
        }

        // Adds one or more variables to a variable registry and returns the registry to provide a fluent syntax.
        public static IAoiVariableRegistry Add(this IAoiVariableRegistry registry, params IAoiVariable[] variables)
        {
            _ = registry.WhenNotNull(nameof(registry));
            _ = variables.WhenNotNullOrEmpty(nameof(variables));

            foreach (var variable in variables)
            {
                registry.AddVariable(variable);
            }

            return registry;
        }
    }
}