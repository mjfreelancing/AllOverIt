using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    // A read-only constant variable that must be initialized at the time of construction.
    public sealed class AoiConstantVariable : AoiVariableBase
    {
        public override double Value { get; }

        // 'referencedVariableNames' is an optional list of variable names that this variable depends on to calculate its value.
        public AoiConstantVariable(string name, double value = default, IEnumerable<string> referencedVariableNames = null) 
            : base(name, referencedVariableNames)
        {
            Value = value;
        }
    }
}