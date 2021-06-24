using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    // TODO: Think about how to best make variables aware of change to avoid potential recalculation. Applies to:
    //       (i) variables dependent on other mutable variables (when a new value is set)
    //       (ii) potentially delegates (they could call out to another provider)

    // Implements a generic read/write variable that has no external dependencies or influence.
    public sealed class MutableVariable : VariableBase, IMutableVariable
    {
        private double _value;

        public override double Value => _value;

        // 'referencedVariableNames' is an optional list of variable names that this variable depends on to calculate its value.
        public MutableVariable(string name, double value = default, IEnumerable<string> referencedVariableNames = null)
            : base(name, referencedVariableNames)
        {
            SetValue(value);
        }

        public void SetValue(double value)
        {
            _value = value;
        }
    }
}