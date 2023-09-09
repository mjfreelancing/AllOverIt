namespace AllOverIt.Evaluator.Variables
{
    /// <summary>A variable that can have its value changed.</summary>
    public sealed class MutableVariable : VariableBase, IMutableVariable
    {
        private double _value;

        /// <summary>The current value of the variable.</summary>
        public override double Value => _value;

        /// <summary>Constructor.</summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The variable's initial value.</param>
        public MutableVariable(string name, double value = default)
            : base(name)
        {
            SetValue(value);
        }

        /// <inheritdoc />
        public void SetValue(double value)
        {
            _value = value;
        }
    }
}