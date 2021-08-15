using AllOverIt.Evaluator.Variables;

namespace AllOverIt.Evaluator.Tests.Variables.Dummies
{
    internal record VariableBaseDummy : VariableBase
    {
        private double _value;

        public override double Value => _value;

        public VariableBaseDummy(string name, double value = default)
          : base(name)
        {
            SetValue(value);
        }

        public void SetValue(double value)
        {
            _value = value;
        }
    }
}
