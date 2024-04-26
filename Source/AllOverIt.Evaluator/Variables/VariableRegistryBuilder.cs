using AllOverIt.Assertion;
using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Specification.Extensions;

namespace AllOverIt.Evaluator.Variables
{
    /// <summary>A builder for an <see cref="IVariableRegistry"/> that will only allow the registry to be
    /// constructed if all variables referenced by all formulae are registered. If none of the variables are
    /// formula based then <see cref="IVariableRegistry"/> can be used directly.</summary>
    public sealed class VariableRegistryBuilder : IVariableRegistryBuilder
    {
        private sealed class PendingRegistrationState
        {
            public List<string> PendingNames { get; } = [];         // Variable names being registered (but cannot due to missing referenced variables)
            public HashSet<string> MissingNames { get; } = [];
        }

        // The PendingRegistrationState input is optional. When not null it will be populated with unregistered and associated referenced variable names that are also not registered.
        private readonly List<Func<PendingRegistrationState, bool>> _pendingRegistrations = [];

        private readonly IVariableRegistry _variableRegistry;
        private readonly IVariableFactory _variableFactory;

        /// <summary>Constructor.</summary>
        public VariableRegistryBuilder()
            : this(new VariableRegistry(), new VariableFactory())
        {
        }

        internal VariableRegistryBuilder(IVariableRegistry variableRegistry, IVariableFactory variableFactory)
        {
            _variableRegistry = variableRegistry.WhenNotNull(nameof(variableRegistry));
            _variableFactory = variableFactory.WhenNotNull(nameof(variableFactory));
        }

        /// <summary>A factory method to create a new <see cref="VariableRegistryBuilder"/> instance. This method primarily exists
        /// so it can be chained with other methods to help provide a more fluent syntax.</summary>
        /// <returns>A new <see cref="VariableRegistryBuilder"/> instance.</returns>
        public static VariableRegistryBuilder Create()
        {
            return new VariableRegistryBuilder();
        }

        /// <inheritdoc />
        public IVariableRegistryBuilder AddConstantVariable(string name, double value)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            var variable = _variableFactory.CreateConstantVariable(name, value);

            _variableRegistry.AddVariable(variable);

            return this;
        }

        /// <inheritdoc />
        public IVariableRegistryBuilder AddDelegateVariable(string name, Func<double> valueResolver)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = valueResolver.WhenNotNull(nameof(valueResolver));

            var variable = _variableFactory.CreateDelegateVariable(name, valueResolver);

            _variableRegistry.AddVariable(variable);

            return this;
        }

        /// <inheritdoc />
        public IVariableRegistryBuilder AddDelegateVariable(string name, Func<IVariableRegistry, double> valueResolver)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = valueResolver.WhenNotNull(nameof(valueResolver));

            var variable = _variableFactory.CreateDelegateVariable(name, () => valueResolver.Invoke(_variableRegistry));

            _variableRegistry.AddVariable(variable);

            return this;
        }

        /// <inheritdoc />
        public IVariableRegistryBuilder AddDelegateVariable(string name, FormulaCompilerResult formulaCompilerResultResolver)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = formulaCompilerResultResolver.WhenNotNull(nameof(formulaCompilerResultResolver));

            return AddDelegateVariable(name, _ => formulaCompilerResultResolver);
        }

        /// <inheritdoc />
        public IVariableRegistryBuilder AddDelegateVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> formulaCompilerResultResolver)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = formulaCompilerResultResolver.WhenNotNull(nameof(formulaCompilerResultResolver));

            return TryRegisterVariable(name, formulaCompilerResultResolver, _variableFactory.CreateDelegateVariable);
        }

        /// <inheritdoc />
        public IVariableRegistryBuilder AddLazyVariable(string name, Func<double> valueResolver, bool threadSafe = false)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = valueResolver.WhenNotNull(nameof(valueResolver));

            var variable = _variableFactory.CreateLazyVariable(name, valueResolver, threadSafe);

            _variableRegistry.AddVariable(variable);

            return this;
        }

        /// <inheritdoc />
        public IVariableRegistryBuilder AddLazyVariable(string name, FormulaCompilerResult formulaCompilerResultResolver, bool threadSafe = false)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = formulaCompilerResultResolver.WhenNotNull(nameof(formulaCompilerResultResolver));

            return AddLazyVariable(name, _ => formulaCompilerResultResolver, threadSafe);
        }

        /// <inheritdoc />
        public IVariableRegistryBuilder AddLazyVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> formulaCompilerResultResolver, bool threadSafe = false)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = formulaCompilerResultResolver.WhenNotNull(nameof(formulaCompilerResultResolver));

            return TryRegisterVariable(
                name,
                formulaCompilerResultResolver,
                (variableName, compilerResult) => _variableFactory.CreateLazyVariable(variableName, compilerResult, threadSafe));
        }

        /// <inheritdoc />
        public IVariableRegistryBuilder AddMutableVariable(string name, double value = default)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            var variable = _variableFactory.CreateMutableVariable(name, value);

            _variableRegistry.AddVariable(variable);

            return this;
        }

        /// <inheritdoc />
        public IVariableRegistry Build()
        {
            var success = TryBuild(out var variableRegistry);

            Throw<VariableRegistryBuilderException>.WhenNot(success, "Cannot build the variable registry due to missing variable references.");

            return variableRegistry;
        }

        /// <inheritdoc />
        public bool TryBuild(out IVariableRegistry variableRegistry)
        {
            ProcessPendingRegistrations(null);

            variableRegistry = _pendingRegistrations.Count == 0
                ? _variableRegistry
                : null;

            return variableRegistry is not null;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<string> GetUnregisteredVariableNames()
        {
            // There may have been pending variables that have since been added as variable types that don't have references,
            // such as constant / mutable variables so we must first process the pending list.
            ProcessPendingRegistrations(null);

            var pendingState = new PendingRegistrationState();

            foreach (var pendingRegistration in _pendingRegistrations)
            {
                pendingRegistration.Invoke(pendingState);
            }

            // Exclude variables in the pending list, just in case they are referenced by another variable that also cannot be resolved.
            // For example:
            //     a = 0
            //     b = a + e * f
            //     d = c - b + g
            //
            // Above, b is pending registration since e and f cannot be resolved - only e, f, c, g will be reported as unregistered
            // despite b not yet being present in the variable registry.
            return pendingState.MissingNames
                .Except(pendingState.PendingNames)
                .AsReadOnlyCollection();
        }

        private VariableRegistryBuilder TryRegisterVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> formulaCompilerResultResolver,
            Func<string, FormulaCompilerResult, IVariable> variableResolver)
        {
            var formulaCompilerResult = formulaCompilerResultResolver.Invoke(_variableRegistry);

            bool TryRegisterVariable(PendingRegistrationState getMissingVariableNames)
            {
                var referencedVariables = formulaCompilerResult.ReferencedVariableNames;

                var missingVariableNames = referencedVariables
                    .Where(referencedVariable => !_variableRegistry.ContainsVariable(referencedVariable))
                    .AsReadOnlyCollection();

                if (missingVariableNames.Count > 0)
                {
                    getMissingVariableNames?.PendingNames.Add(name);
                    getMissingVariableNames?.MissingNames.UnionWith(missingVariableNames);

                    return false;
                }

                var variable = variableResolver.Invoke(name, formulaCompilerResult);

                _variableRegistry.AddVariable(variable);

                return true;
            }

            if (!TryRegisterVariable(null))
            {
                // Delay the registration until Build() is called
                _pendingRegistrations.Add(TryRegisterVariable);
            }

            return this;
        }

        private void ProcessPendingRegistrations(PendingRegistrationState pendingState)
        {
            if (_pendingRegistrations.Count == 0)
            {
                return;
            }

            Func<PendingRegistrationState, bool> next;

            do
            {
                next = _pendingRegistrations.FirstOrDefault(pendingItem => pendingItem.Invoke(pendingState));

                if (next is not null)
                {
                    _pendingRegistrations.Remove(next);
                }
            } while (next is not null);
        }
    }
}