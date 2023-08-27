using System;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    public interface IVariableRegistryBuilder
    {
        VariableRegistryBuilder AddConstantVariable(string name, double value = 0);
        VariableRegistryBuilder AddConstantVariable(string name, Func<IVariableRegistry, double> valueResolver);
        VariableRegistryBuilder AddDelegateVariable(string name, Func<double> valueResolver);
        VariableRegistryBuilder AddDelegateVariable(string name, Func<IVariableRegistry, double> valueResolver);
        VariableRegistryBuilder AddDelegateVariable(string name, FormulaCompilerResult formulaCompilerResult);
        VariableRegistryBuilder AddDelegateVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> formulaCompilerResultResolver);
        VariableRegistryBuilder AddLazyVariable(string name, Func<double> valueResolver, bool threadSafe = false);
        VariableRegistryBuilder AddLazyVariable(string name, Func<IVariableRegistry, double> valueResolver, bool threadSafe = false);
        VariableRegistryBuilder AddLazyVariable(string name, FormulaCompilerResult formulaCompilerResult, bool threadSafe = false);
        VariableRegistryBuilder AddLazyVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> formulaCompilerResultResolver, bool threadSafe = false);
        VariableRegistryBuilder AddMutableVariable(string name, double value = 0);
        VariableRegistryBuilder AddMutableVariable(string name, Func<IVariableRegistry, double> value);
        IVariableRegistry Build();
        bool TryBuild(out IVariableRegistry variableRegistry);
        IReadOnlyCollection<string> GetUnregisteredVariableNames();
    }
}