using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;

namespace AllOverIt.Filtering.Tests.Extensions
{
    public class FilterExtensionsFixture : FixtureBase
    {
        private class DummyClass
        {
            public int? Prop1 { get; }
            public string Prop2 { get; }
        }

        private class DummyFilter
        {
            public class Prop1Filter
            {
                public EqualTo<int?> EqualTo { get; set; } = new();
                public In<int?> In { get; set; } = new();
            }

            public class Prop2Filter
            {
                public EqualTo<string> EqualTo { get; set; } = new();
                public In<string> In { get; set; } = new();
            }

            public Prop1Filter Prop1 { get; init; } = new();
            public Prop2Filter Prop2 { get; init; } = new();
        }



    }
}
