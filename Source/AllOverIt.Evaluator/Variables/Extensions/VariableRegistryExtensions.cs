using AllOverIt.Helpers;
using System;

namespace AllOverIt.Evaluator.Variables.Extensions
{
    public static class VariableRegistryExtensions
    {
        // Adds a variable to a variable registry and returns the registry to provide a fluent syntax.
        public static IVariableRegistry Add(this IVariableRegistry registry, IVariable variable)
        {
            _ = registry.WhenNotNull(nameof(registry));
            _ = variable.WhenNotNull(nameof(variable));

            registry.AddVariable(variable);

            return registry;
        }

        // Adds one or more variables to a variable registry and returns the registry to provide a fluent syntax.
        public static IVariableRegistry Add(this IVariableRegistry registry, params IVariable[] variables)
        {
            _ = registry.WhenNotNull(nameof(registry));
            _ = variables.WhenNotNullOrEmpty(nameof(variables));

            foreach (var variable in variables)
            {
                registry.AddVariable(variable);
            }

            return registry;
        }




        public static IVariable AddConstantVariable(this IVariableRegistry variableRegistry, string name, double value = default)
        {
            var variable = new ConstantVariable(name, value);
            variableRegistry.AddVariable(variable);

            return variable;
        }

        public static IVariable AddMutableVariable(this IVariableRegistry variableRegistry, string name, double value = default)
        {
            var variable = new MutableVariable(name, value);
            variableRegistry.AddVariable(variable);

            return variable;
        }

        public static IVariable AddDelegateVariable(this IVariableRegistry variableRegistry, string name, Func<double> value = default)
        {
            var variable = new DelegateVariable(name, value);
            variableRegistry.AddVariable(variable);

            return variable;
        }

        public static IVariable AddDelegateVariable(this IVariableRegistry variableRegistry, string name, FormulaCompilerResult compilerResult)
        {
            var variable = new DelegateVariable(name, compilerResult);
            variableRegistry.AddVariable(variable);

            return variable;
        }

        public static IVariable AddLazyVariable(this IVariableRegistry variableRegistry, string name, Func<double> value = default)
        {
            var variable = new LazyVariable(name, value);
            variableRegistry.AddVariable(variable);

            return variable;
        }

        public static IVariable AddLazyVariable(this IVariableRegistry variableRegistry, string name, FormulaCompilerResult compilerResult)
        {
            var variable = new LazyVariable(name, compilerResult);
            variableRegistry.AddVariable(variable);

            return variable;
        }


    }
}