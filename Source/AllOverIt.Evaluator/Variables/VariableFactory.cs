using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables
{
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
            var selectedVariableNames = variableNames.WhenNotNullOrEmpty(nameof(variableNames))?.AsReadOnlyCollection();

            var allVariables = from item in variableRegistry.Variables
                               let variable = item.Value
                               select variable.Value;
            
            var sumValues = selectedVariableNames == null
              ? allVariables
              : selectedVariableNames.Select(variableRegistry.GetValue);

            return new DelegateVariable(name, () => sumValues.Sum());
        }
    }
}