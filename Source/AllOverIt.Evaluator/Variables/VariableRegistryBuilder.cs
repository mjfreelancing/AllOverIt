using AllOverIt.Assertion;
using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables
{
    public sealed class VariableRegistryBuilder : IVariableRegistryBuilder
    {
        private sealed class PendingRegistrationState
        {
            public IList<string> PendingNames { get; } = new List<string>();            // Variable names being registered (but cannot due to missing referenced variables)
            public HashSet<string> MissingNames { get; } = new HashSet<string>();
        }

        // The PendingRegistrationState input is optional. When not null it will be populated with unregistered and associated referenced variable names that are also not registered.
        private readonly IList<Func<PendingRegistrationState, bool>> _pendingRegistrations = new List<Func<PendingRegistrationState, bool>>();

        private readonly IVariableRegistry _variableRegistry;
        private readonly IVariableFactory _variableFactory;

        public VariableRegistryBuilder()
            : this(new VariableRegistry(), new VariableFactory())
        {
        }

        internal VariableRegistryBuilder(IVariableRegistry variableRegistry, IVariableFactory variableFactory)
        {
            _variableRegistry = variableRegistry.WhenNotNull(nameof(variableRegistry));
            _variableFactory = variableFactory.WhenNotNull(nameof(variableFactory));
        }

        public static VariableRegistryBuilder Create()
        {
            return new VariableRegistryBuilder();
        }

        public VariableRegistryBuilder AddConstantVariable(string name, double value = default)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            return AddConstantVariable(name, _ => value);
        }

        public VariableRegistryBuilder AddConstantVariable(string name, Func<IVariableRegistry, double> valueResolver)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = valueResolver.WhenNotNull(nameof(valueResolver));

            var variable = _variableFactory.CreateConstantVariable(name, valueResolver.Invoke(_variableRegistry));

            _variableRegistry.AddVariable(variable);

            return this;
        }

        public VariableRegistryBuilder AddDelegateVariable(string name, Func<double> valueResolver)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = valueResolver.WhenNotNull(nameof(valueResolver));

            var variable = _variableFactory.CreateDelegateVariable(name, valueResolver);

            _variableRegistry.AddVariable(variable);

            return this;
        }

        public VariableRegistryBuilder AddDelegateVariable(string name, Func<IVariableRegistry, double> valueResolver)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = valueResolver.WhenNotNull(nameof(valueResolver));

            var variable = _variableFactory.CreateDelegateVariable(name, () => valueResolver.Invoke(_variableRegistry));

            _variableRegistry.AddVariable(variable);

            return this;
        }

        public VariableRegistryBuilder AddDelegateVariable(string name, FormulaCompilerResult formulaCompilerResultResolver)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = formulaCompilerResultResolver.WhenNotNull(nameof(formulaCompilerResultResolver));

            return AddDelegateVariable(name, _ => formulaCompilerResultResolver);
        }

        public VariableRegistryBuilder AddDelegateVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> formulaCompilerResultResolver)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = formulaCompilerResultResolver.WhenNotNull(nameof(formulaCompilerResultResolver));

            return TryRegisterVariable(name, formulaCompilerResultResolver, _variableFactory.CreateDelegateVariable);
        }

        public VariableRegistryBuilder AddLazyVariable(string name, Func<double> valueResolver, bool threadSafe = false)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = valueResolver.WhenNotNull(nameof(valueResolver));

            var variable = _variableFactory.CreateLazyVariable(name, valueResolver, threadSafe);

            _variableRegistry.AddVariable(variable);

            return this;
        }

        public VariableRegistryBuilder AddLazyVariable(string name, Func<IVariableRegistry, double> valueResolver, bool threadSafe = false)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = valueResolver.WhenNotNull(nameof(valueResolver));

            var variable = _variableFactory.CreateLazyVariable(name, () => valueResolver.Invoke(_variableRegistry), threadSafe);

            _variableRegistry.AddVariable(variable);

            return this;
        }

        public VariableRegistryBuilder AddLazyVariable(string name, FormulaCompilerResult formulaCompilerResultResolver, bool threadSafe = false)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = formulaCompilerResultResolver.WhenNotNull(nameof(formulaCompilerResultResolver));

            return AddLazyVariable(name, _ => formulaCompilerResultResolver, threadSafe);
        }

        public VariableRegistryBuilder AddLazyVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> formulaCompilerResultResolver, bool threadSafe = false)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = formulaCompilerResultResolver.WhenNotNull(nameof(formulaCompilerResultResolver));

            return TryRegisterVariable(
                name,
                formulaCompilerResultResolver,
                (variableName, compilerResult) => _variableFactory.CreateLazyVariable(variableName, compilerResult, threadSafe));
        }

        public VariableRegistryBuilder AddMutableVariable(string name, double value = default)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            return AddMutableVariable(name, _ => value);
        }

        public VariableRegistryBuilder AddMutableVariable(string name, Func<IVariableRegistry, double> valueResolver)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = valueResolver.WhenNotNull(nameof(valueResolver));

            var variable = _variableFactory.CreateMutableVariable(name, valueResolver.Invoke(_variableRegistry));

            _variableRegistry.AddVariable(variable);

            return this;
        }

        public IVariableRegistry Build()
        {
            var success = TryBuild(out var variableRegistry);

            Throw<VariableRegistryBuilderException>.WhenNot(success, "Cannot build the variable registry due to missing variable references.");

            return variableRegistry;
        }

        public bool TryBuild(out IVariableRegistry variableRegistry)
        {
            ProcessPendingRegistrations(null);

            variableRegistry = _pendingRegistrations.Count == 0
                ? _variableRegistry
                : null;

            return variableRegistry is not null;
        }

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