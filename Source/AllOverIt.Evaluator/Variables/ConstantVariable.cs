namespace AllOverIt.Evaluator.Variables
{
    // A read-only constant variable that must be initialized at the time of construction.
    public sealed record ConstantVariable : VariableBase
    {
        public override double Value { get; }

        public ConstantVariable(string name, double value = default) 
            : base(name)
        {
            Value = value;
        }
    }
}