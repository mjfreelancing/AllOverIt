namespace DelegateEvaluation
{
    internal class Factorial
    {
        public uint Value { get; set; }

        public double Calculate()
        {
            return DoCalculate(Value);
        }

        private double DoCalculate(uint value)
        {
            return value <= 1
              ? 1.0d
              : value * DoCalculate(value - 1);
        }
    }
}
