namespace AllOverIt.Evaluator.Variables
{
    /// <summary>A read-only constant variable that must be initialized at the time of construction.</summary>
    public sealed record ConstantVariable : VariableBase
    {
        /// <summary>The variable's value.</summary>
        public override double Value { get; }

        /// <summary>Constructor.</summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The variable's value.</param>
        public ConstantVariable(string name, double value = default) 
            : base(name)
        {
            Value = value;
        }
    }
}