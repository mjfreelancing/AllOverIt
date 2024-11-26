using AllOverIt.Assertion;

namespace AllOverIt.Evaluator.Variables.Extensions
{
    /// <summary>Provides a variety of <see cref="IVariableRegistry"/> extensions.</summary>
    public static class VariableRegistryExtensions
    {
        /// <summary>Adds a variable to a variable registry.</summary>
        /// <param name="registry">The registry to add a variable to.</param>
        /// <param name="variable">The variable to add to the registry.</param>
        /// <returns>The same registry to provide a fluent syntax.</returns>
        public static IVariableRegistry Add(this IVariableRegistry registry, IVariable variable)
        {
            _ = registry.WhenNotNull();
            _ = variable.WhenNotNull();

            registry.AddVariable(variable);

            return registry;
        }

        /// <summary>Adds one or more variables to a variable registry.</summary>
        /// <param name="registry">The registry to add the variables to.</param>
        /// <param name="variables">The variables to add to the registry.</param>
        /// <returns>The same registry to provide a fluent syntax.</returns>
        public static IVariableRegistry Add(this IVariableRegistry registry, params IVariable[] variables)
        {
            _ = registry.WhenNotNull();
            _ = variables.WhenNotNullOrEmpty();

            foreach (var variable in variables)
            {
                registry.AddVariable(variable);
            }

            return registry;
        }

        /// <summary>Creates, and adds, a new <see cref="ConstantVariable"/> to the variable registry.</summary>
        /// <param name="registry">The variable registry to add the newly created variable to.</param>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="value">The constant value to be assigned to the variable.</param>
        /// <returns>The new variable instance.</returns>
        public static IVariable AddConstantVariable(this IVariableRegistry registry, string name, double value)
        {
            _ = registry.WhenNotNull();
            _ = name.WhenNotNullOrEmpty();

            var variable = new ConstantVariable(name, value);
            registry.AddVariable(variable);

            return variable;
        }

        /// <summary>Creates, and adds, a new <see cref="MutableVariable"/> to the variable registry.</summary>
        /// <param name="registry">The variable registry to add the newly created variable to.</param>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="value">The initial value to be assigned to the variable.</param>
        /// <returns>The new variable instance.</returns>
        public static IMutableVariable AddMutableVariable(this IVariableRegistry registry, string name, double value = default)
        {
            _ = registry.WhenNotNull();
            _ = name.WhenNotNullOrEmpty();

            var variable = new MutableVariable(name, value);
            registry.AddVariable(variable);

            return variable;
        }

        /// <summary>Creates, and adds, a new <see cref="DelegateVariable"/> to the variable registry.</summary>
        /// <param name="registry">The variable registry to add the newly created variable to.</param>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="valueResolver">The initial delegate to be assigned to the variable.</param>
        /// <returns>The new variable instance.</returns>
        public static IVariable AddDelegateVariable(this IVariableRegistry registry, string name, Func<double> valueResolver)
        {
            _ = registry.WhenNotNull();
            _ = name.WhenNotNullOrEmpty();
            _ = valueResolver.WhenNotNull();

            var variable = new DelegateVariable(name, valueResolver);
            registry.AddVariable(variable);

            return variable;
        }

        /// <summary>Creates, and adds, a new <see cref="DelegateVariable"/> to the variable registry.</summary>
        /// <param name="registry">The variable registry to add the newly created variable to.</param>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="formulaCompilerResult">The compiled result of a formula. The associated resolver will be assigned to the
        /// new variable's delegate.</param>
        /// <returns>The new variable instance.</returns>
        public static IVariable AddDelegateVariable(this IVariableRegistry registry, string name, FormulaCompilerResult formulaCompilerResult)
        {
            _ = registry.WhenNotNull();
            _ = name.WhenNotNullOrEmpty();
            _ = formulaCompilerResult.WhenNotNull();

            var variable = new DelegateVariable(name, formulaCompilerResult);
            registry.AddVariable(variable);

            return variable;
        }

        /// <summary>Creates, and adds, a new <see cref="LazyVariable"/> to the variable registry.</summary>
        /// <param name="registry">The variable registry to add the newly created variable to.</param>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="valueResolver">The initial lazily-evaluated delegate to be assigned to the variable.</param>
        /// <returns>The new variable instance.</returns>
        public static IVariable AddLazyVariable(this IVariableRegistry registry, string name, Func<double> valueResolver)
        {
            _ = registry.WhenNotNull();
            _ = name.WhenNotNullOrEmpty();
            _ = valueResolver.WhenNotNull();

            var variable = new LazyVariable(name, valueResolver);
            registry.AddVariable(variable);

            return variable;
        }

        /// <summary>Creates, and adds, a new <see cref="LazyVariable"/> to the variable registry.</summary>
        /// <param name="registry">The variable registry to add the newly created variable to.</param>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="formulaCompilerResult">The compiled result of a formula. The associated resolver will be assigned to the
        /// new variable's lazily-evaluated delegate.</param>
        /// <returns>The new variable instance.</returns>
        public static IVariable AddLazyVariable(this IVariableRegistry registry, string name, FormulaCompilerResult formulaCompilerResult)
        {
            _ = registry.WhenNotNull();
            _ = name.WhenNotNullOrEmpty();

            var variable = new LazyVariable(name, formulaCompilerResult);
            registry.AddVariable(variable);

            return variable;
        }
    }
}