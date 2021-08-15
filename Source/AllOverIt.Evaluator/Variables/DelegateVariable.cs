using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Helpers;
using System;

namespace AllOverIt.Evaluator.Variables
{
    // Implements a read-only delegate based variable. Unlike ConstantVariable this variable type may change value between consecutive
    // reads depending on the delegate's implementation.
    public sealed record DelegateVariable : VariableBase
    {
        private readonly Func<double> _valueResolver;

        public override double Value => _valueResolver.Invoke();

        // 'referencedVariableNames' is an optional list of variable names that this variable depends on to calculate its value.
        public DelegateVariable(string name, Func<double> valueResolver)
            : base(name)
        {
            _valueResolver = valueResolver.WhenNotNull(nameof(valueResolver));
        }

        public DelegateVariable(string name, FormulaCompilerResult compilerResult)
            : this(name, compilerResult.Resolver)
        {
            ReferencedVariables = compilerResult.GetReferencedVariables();
        }
    }
}