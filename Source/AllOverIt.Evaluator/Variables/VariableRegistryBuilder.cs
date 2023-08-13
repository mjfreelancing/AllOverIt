using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables
{
    public sealed class VariableRegistryBuilder : IVariableRegistryBuilder
    {
        // The List<string> input is an optional collection that will be populated with referenced variable names that are not registered.
        private readonly IList<Func<HashSet<string>, bool>> _pendingRegistrations = new List<Func<HashSet<string>, bool>>();

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
            return AddConstantVariable(name, _ => value);
        }

        public VariableRegistryBuilder AddConstantVariable(string name, Func<IVariableRegistry, double> valueResolver)
        {
            var variable = _variableFactory.CreateConstantVariable(name, valueResolver.Invoke(_variableRegistry));

            _variableRegistry.AddVariable(variable);

            return this;
        }

        public VariableRegistryBuilder AddDelegateVariable(string name, Func<double> valueResolver)
        {
            var variable = _variableFactory.CreateDelegateVariable(name, valueResolver);

            _variableRegistry.AddVariable(variable);

            return this;
        }

        public VariableRegistryBuilder AddDelegateVariable(string name, Func<IVariableRegistry, double> valueResolver)
        {
            var variable = _variableFactory.CreateDelegateVariable(name, () => valueResolver.Invoke(_variableRegistry));

            _variableRegistry.AddVariable(variable);

            return this;
        }

        public VariableRegistryBuilder AddDelegateVariable(string name, FormulaCompilerResult compilerResult)
        {
            return AddDelegateVariable(name, _ => compilerResult);
        }

        public VariableRegistryBuilder AddDelegateVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> compilerResultResolver)
        {
            return TryRegisterVariable(name, compilerResultResolver, _variableFactory.CreateDelegateVariable);
        }

        public VariableRegistryBuilder AddLazyVariable(string name, Func<double> valueResolver, bool threadSafe = false)
        {
            var variable = _variableFactory.CreateLazyVariable(name, valueResolver, threadSafe);

            _variableRegistry.AddVariable(variable);

            return this;
        }

        public VariableRegistryBuilder AddLazyVariable(string name, Func<IVariableRegistry, double> valueResolver, bool threadSafe = false)
        {
            var variable = _variableFactory.CreateLazyVariable(name, () => valueResolver.Invoke(_variableRegistry), threadSafe);

            _variableRegistry.AddVariable(variable);

            return this;
        }

        public VariableRegistryBuilder AddLazyVariable(string name, FormulaCompilerResult compilerResult, bool threadSafe = false)
        {
            return AddLazyVariable(name, _ => compilerResult, threadSafe);
        }

        public VariableRegistryBuilder AddLazyVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> compilerResultResolver, bool threadSafe = false)
        {
            return TryRegisterVariable(
                name,
                compilerResultResolver,
                (variableName, compilerResult) => _variableFactory.CreateLazyVariable(variableName, compilerResult, threadSafe));
        }

        public VariableRegistryBuilder AddMutableVariable(string name, double value = default)
        {
            return AddMutableVariable(name, _ => value);
        }

        public VariableRegistryBuilder AddMutableVariable(string name, Func<IVariableRegistry, double> value)
        {
            var variable = _variableFactory.CreateMutableVariable(name, value.Invoke(_variableRegistry));

            _variableRegistry.AddVariable(variable);

            return this;
        }

        public bool TryBuild(out IVariableRegistry variableRegistry)
        {
            ProcessPendingRegistrations(null);

            variableRegistry = _pendingRegistrations.Count == 0
                ? _variableRegistry
                : null;

            return variableRegistry is not null;
        }

        public IVariableRegistry Build()
        {
            var success = TryBuild(out var variableRegistry);

            // TODO: Custom exception
            Throw<InvalidOperationException>.WhenNot(success, "Cannot build the variable registry due to missing variable referrences.");

            return variableRegistry;
        }

        public IReadOnlyCollection<string> GetUnregisteredVariableNames()
        {
            var missingVariableNames = new HashSet<string>();

            foreach (var pendingRegistration in _pendingRegistrations)
            {
                pendingRegistration.Invoke(missingVariableNames);
            }

            return missingVariableNames;
        }

        private VariableRegistryBuilder TryRegisterVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> compilerResultResolver,
          Func<string, FormulaCompilerResult, IVariable> variableResolver)
        {
            var compilerResult = compilerResultResolver.Invoke(_variableRegistry);

            bool TryRegisterVariable(HashSet<string> getMissingVariableNames)
            {
                var referencedVariables = compilerResult.ReferencedVariableNames;

                var missingVariableNames = referencedVariables
                    .Where(referencedVariable => !_variableRegistry.ContainsVariable(referencedVariable))
                    .AsReadOnlyCollection();

                if (missingVariableNames.Count > 0)
                {
                    getMissingVariableNames?.AddRange(missingVariableNames);

                    return false;
                }

                var variable = variableResolver.Invoke(name, compilerResult);

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

        private void ProcessPendingRegistrations(HashSet<string> missingVariableNmes)
        {
            if (_pendingRegistrations.Count == 0)
            {
                return;
            }

            Func<HashSet<string>, bool> next;

            do
            {
                next = _pendingRegistrations.FirstOrDefault(pendingItem => pendingItem.Invoke(missingVariableNmes));

                if (next is not null)
                {
                    _pendingRegistrations.Remove(next);
                }
            } while (next is not null);
        }
    }
}