using System;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    public interface IVariableFactory
    {
        IVariableRegistry CreateVariableRegistry();

        IMutableVariable CreateMutableVariable(string name, double value = default, IEnumerable<string> referencedVariableNames = null);

        IVariable CreateConstantVariable(string name, double value = default, IEnumerable<string> referencedVariableNames = null);

        IVariable CreateDelegateVariable(string name, Func<double> func, IEnumerable<string> referencedVariableNames = null);

        ILazyVariable CreateLazyVariable(string name, Func<double> func, IEnumerable<string> referencedVariableNames = null, bool threadSafe = false);

        IVariable CreateAggregateVariable(string name, params Func<double>[] funcs);

        IVariable CreateAggregateVariable(string name, IVariableRegistry variableRegistry, IEnumerable<string> variableNames = null);
    }
}