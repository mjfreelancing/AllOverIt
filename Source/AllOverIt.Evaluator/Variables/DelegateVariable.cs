using AllOverIt.Helpers;
using System;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    // Implements a read-only delegate based variable. Unlike AoiConstantVariable this variable type may change value between consecutive
    // reads depending on the delegate's implementation.
    public sealed class DelegateVariable : VariableBase
    {
        private Func<double> ValueResolver { get; }
        public override double Value => ValueResolver.Invoke();

        // 'referencedVariableNames' is an optional list of variable names that this variable depends on to calculate its value.
        public DelegateVariable(string name, Func<double> valueResolver, IEnumerable<string> referencedVariableNames = null)
            : base(name, referencedVariableNames)
        {
            ValueResolver = valueResolver.WhenNotNull(nameof(valueResolver));
        }
    }
}