using AllOverIt.DependencyInjection.Tests.Types;

namespace AllOverIt.DependencyInjection.Tests
{
    public sealed class ConcreteClassJ : IBaseInterface3
    {
        public int Value { get; }

        public ConcreteClassJ(int value)
        {
            Value = value - 100;
        }
    }
}