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
        VariableRegistryBuilder AddDelegateVariable(string name, FormulaCompilerResult compilerResult);
        VariableRegistryBuilder AddDelegateVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> compilerResultResolver);
        VariableRegistryBuilder AddLazyVariable(string name, Func<double> valueResolver, bool threadSafe = false);
        VariableRegistryBuilder AddLazyVariable(string name, Func<IVariableRegistry, double> valueResolver, bool threadSafe = false);
        VariableRegistryBuilder AddLazyVariable(string name, FormulaCompilerResult compilerResult, bool threadSafe = false);
        VariableRegistryBuilder AddLazyVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> compilerResultResolver, bool threadSafe = false);
        VariableRegistryBuilder AddMutableVariable(string name, double value = 0);
        VariableRegistryBuilder AddMutableVariable(string name, Func<IVariableRegistry, double> value);
        bool TryBuild(out IVariableRegistry variableRegistry);
        IVariableRegistry Build();
        IReadOnlyCollection<string> GetUnregisteredVariableNames();
    }
}