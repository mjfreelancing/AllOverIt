using AllOverIt.Evaluator.Variables;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Tests.Variables.Dummies
{
    internal record VariableBaseDummy : VariableBase
    {
        private double _value;

        public override double Value => _value;

        public VariableBaseDummy(string name, double value = default, IEnumerable<string> referencedVariableNames = null)
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
