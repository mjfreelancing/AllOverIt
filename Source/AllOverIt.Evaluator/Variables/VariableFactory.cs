using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables
{
    /// <summary>A factory for creating variables.</summary>
    public sealed class VariableFactory : IVariableFactory
    {
        /// <inheritdoc />
        public IVariableRegistry CreateVariableRegistry()
        {
            return new VariableRegistry();
        }

        /// <inheritdoc />
        public IVariable CreateConstantVariable(string name, double value = default)
        {
            return new ConstantVariable(name, value);
        }

        /// <inheritdoc />
        public IMutableVariable CreateMutableVariable(string name, double value = default)
        {
            return new MutableVariable(name, value);
        }

        /// <inheritdoc />
        public IVariable CreateDelegateVariable(string name, Func<double> valueResolver)
        {
            return new DelegateVariable(name, valueResolver);
        }

        /// <inheritdoc />
        public ILazyVariable CreateLazyVariable(string name, Func<double> valueResolver, bool threadSafe = false)
        {
            return new LazyVariable(name, valueResolver, threadSafe);
        }

        /// <inheritdoc />
        public IVariable CreateAggregateVariable(string name, params Func<double>[] valueResolvers)
        {
            _ = valueResolvers.WhenNotNullOrEmpty(nameof(valueResolvers));

            var sumValues = valueResolvers
                .Select(resolver => resolver.Invoke())
                .Sum();

            return new DelegateVariable(name, () => sumValues);
        }

        /// <inheritdoc />
        public IVariable CreateAggregateVariable(string name, IVariableRegistry variableRegistry, IEnumerable<string> variableNames = null)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = variableRegistry.WhenNotNull(nameof(variableRegistry));

            // Can be null, but cannot be empty when it isn't
            var selectedVariableNames = variableNames?.WhenNotEmpty(nameof(variableNames));

            var allVariables = from item in variableRegistry
                               let variable = item.Value
                               select variable.Value;
            
            var sumValues = selectedVariableNames == null
              ? allVariables
              : selectedVariableNames.Select(variableRegistry.GetValue);

            return new DelegateVariable(name, () => sumValues.Sum());
        }
    }
}