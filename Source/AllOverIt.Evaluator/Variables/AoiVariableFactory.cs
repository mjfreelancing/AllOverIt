using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Variables
{
    public sealed class AoiVariableFactory : IAoiVariableFactory
    {
        public IAoiVariableRegistry CreateVariableRegistry()
        {
            return new AoiVariableRegistry();
        }

        public IAoiMutableVariable CreateMutableVariable(string name, double value = default, IEnumerable<string> referencedVariableNames = null)
        {
            return new AoiMutableVariable(name, value, referencedVariableNames);
        }

        public IAoiVariable CreateConstantVariable(string name, double value = default, IEnumerable<string> referencedVariableNames = null)
        {
            return new AoiConstantVariable(name, value, referencedVariableNames);
        }

        public IAoiVariable CreateDelegateVariable(string name, Func<double> func, IEnumerable<string> referencedVariableNames = null)
        {
            return new AoiDelegateVariable(name, func, referencedVariableNames);
        }

        public IAoiLazyVariable CreateLazyVariable(string name, Func<double> func, IEnumerable<string> referencedVariableNames = null, bool threadSafe = false)
        {
            return new AoiLazyVariable(name, func, referencedVariableNames, threadSafe);
        }

        public IAoiVariable CreateAggregateVariable(string name, params Func<double>[] funcs)
        {
            _ = funcs.WhenNotNullOrEmpty(nameof(funcs));

            var sumValues = from func in funcs
                            select func.Invoke();

            return new AoiDelegateVariable(name, () => sumValues.Sum());
        }

        // Creates a read-only variable that calculates the sum of registered variables.
        // 'variableNames' contains the variable names to be aggregated. If this parameter is null then all variables are aggregated. It cannot be an empty list.
        public IAoiVariable CreateAggregateVariable(string name, IAoiVariableRegistry variableRegistry, IEnumerable<string> variableNames = null)
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

            return new AoiDelegateVariable(name, () => sumValues.Sum());
        }
    }
}