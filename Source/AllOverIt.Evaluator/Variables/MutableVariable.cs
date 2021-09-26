namespace AllOverIt.Evaluator.Variables
{
    // TODO: Think about how to best make variables aware of change to avoid potential recalculation. Applies to:
    //       (i) variables dependent on other mutable variables (when a new value is set)
    //       (ii) potentially delegates (they could call out to another provider)



    /// <summary>A variable that can have its value changed.</summary>
    public sealed record MutableVariable : VariableBase, IMutableVariable
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