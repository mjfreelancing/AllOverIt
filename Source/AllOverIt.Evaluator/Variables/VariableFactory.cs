using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables
{
    public sealed class VariableFactory : IVariableFactory
    {
        public IVariableRegistry CreateVariableRegistry()
        {
            return new VariableRegistry();
        }

        public IMutableVariable CreateMutableVariable(string name, double value = default)
        {
            return new MutableVariable(name, value);
        }

        public IVariable CreateConstantVariable(string name, double value = default)
        {
            return new ConstantVariable(name, value);
        }

        public IVariable CreateDelegateVariable(string name, Func<double> func)
        {
            return new DelegateVariable(name, func);
        }

        public ILazyVariable CreateLazyVariable(string name, Func<double> func, bool threadSafe = false)
        {
            return new LazyVariable(name, func, threadSafe);
        }

        public IVariable CreateAggregateVariable(string name, params Func<double>[] funcs)
        {
            _ = funcs.WhenNotNullOrEmpty(nameof(funcs));

            var sumValues = from func in funcs
                            select func.Invoke();

            return new DelegateVariable(name, () => sumValues.Sum());
        }

        // Creates a read-only variable that calculates the sum of registered variables.
        // 'variableNames' contains the variable names to be aggregated. If this parameter is null then all variables are aggregated. It cannot be an empty list.
        public IVariable CreateAggregateVariable(string name, IVariableRegistry variableRegistry, IEnumerable<string> variableNames = null)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));
            _ = variableRegistry.WhenNotNull(nameof(variableRegistry));
            var filteredVariableNames = variableNames?.WhenNotNullOrEmpty(nameof(variableNames)).AsReadOnlyCollection();

            var allVariables = from item in variableRegistry.Variables
                               let variable = item.Value
                               select variable.Value;
            
            var sumValues = filteredVariableNames == null
              ? allVariables
              : filteredVariableNames.Select(variableRegistry.GetValue);

            return new DelegateVariable(name, () => sumValues.Sum());
        }
    }
}