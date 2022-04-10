namespace AllOverIt.DependencyInjection.Tests.Types
{
    public sealed class ConcreteClassH : IBaseInterface3
    {
        public int Value { get; }

        public ConcreteClassH(int value)
        {
            Value = value;
        }
    }
}