using System;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    public interface IAoiVariableFactory
    {
        IAoiVariableRegistry CreateVariableRegistry();

        IAoiMutableVariable CreateMutableVariable(string name, double value = default, IEnumerable<string> referencedVariableNames = null);

        IAoiVariable CreateConstantVariable(string name, double value = default, IEnumerable<string> referencedVariableNames = null);

        IAoiVariable CreateDelegateVariable(string name, Func<double> func, IEnumerable<string> referencedVariableNames = null);

        IAoiLazyVariable CreateLazyVariable(string name, Func<double> func, IEnumerable<string> referencedVariableNames = null, bool threadSafe = false);

        IAoiVariable CreateAggregateVariable(string name, params Func<double>[] funcs);

        IAoiVariable CreateAggregateVariable(string name, IAoiVariableRegistry variableRegistry, IEnumerable<string> variableNames = null);
    }
}