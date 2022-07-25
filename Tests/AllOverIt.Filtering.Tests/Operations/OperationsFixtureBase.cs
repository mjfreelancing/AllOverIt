using AllOverIt.Fixture;

namespace AllOverIt.Filtering.Tests.Operations
{
    public class OperationsFixtureBase : FixtureBase
    {
        internal sealed class DummyClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        internal DummyClass Model { get; }

        public OperationsFixtureBase()
        {
            Model = Create<DummyClass>();
        }
    }
}
