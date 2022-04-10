namespace AllOverIt.DependencyInjection.Tests.Types
{
    public sealed class ConcreteClassI : IBaseInterface3
    {
        public int Value { get; }

        public ConcreteClassI(int value)
        {
            Value = value * 100;
        }
    }
}