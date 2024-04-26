using AllOverIt.Filtering.Extensions;
using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using AllOverIt.Patterns.Specification.Extensions;
using FluentAssertions;
using System.Linq;

namespace AllOverIt.Filtering.Tests.Extensions
{
    public class QueryableExtensionsFixture : FixtureBase
    {
        private class DummyClass
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
        }

        private class DummyFilter
        {
            public class Prop1Filter
            {
                public In<int> In { get; set; } = new();
                public NotIn<int?> NotIn { get; set; } = new();
            }

            public class Prop2Filter
            {
                public EqualTo<string> EqualTo { get; set; } = new();
                public Contains Contains { get; set; } = new();
            }

            public Prop1Filter Prop1 { get; init; } = new();
            public Prop2Filter Prop2 { get; init; } = new();
        }

        public class ApplyFilter : QueryableExtensionsFixture
        {
            [Fact]
            public void Should_Apply_Filter()
            {
                var data = CreateMany<DummyClass>(20);

                var firstHalf = data.Take(10).ToList();
                var secondHalf = data.Skip(10).ToList();

                var firstGroup = new[] { firstHalf[0], firstHalf[9] };
                var secondGroup = new[] { secondHalf[0], secondHalf[9] };

                var expected = firstGroup.Concat(secondGroup).ToList();

                var dataFilter = new DummyFilter
                {
                    Prop1 =
                    {
                        // Will result in first and last element being selected
                        In = firstHalf.Select(item => item.Prop1).ToList(),
                        NotIn = firstHalf.Skip(1).Take(8).Select(item => (int?)item.Prop1).ToList()
                    },
                    Prop2 =
                    {
                        // Will result in first and last element being selected
                        EqualTo = secondGroup[0].Prop2,
                        Contains = secondGroup[1].Prop2[..8],
                    }
                };

                var actual = data
                    .AsQueryable()
                    .ApplyFilter(dataFilter, (specificationBuilder, filterBuilder) =>
                    {
                        // Demonstrating mixing specifications with direct filter builder operations
                        var spec1 = specificationBuilder.Create(model => model.Prop2, filter => filter.Prop2.EqualTo);
                        var spec2 = specificationBuilder.Create(model => model.Prop2, filter => filter.Prop2.Contains);
                        var spec3 = spec1.Or(spec2);

                        // Sample resultant filter (as a query string)
                        // (((99, 218, 154, 33, 212, 115, 142, 11, 22, 32) Contains Prop1 AND NOT ((218, 154, 33, 212, 115, 142, 11, 22) Contains Prop1)) OR
                        // (( Compare (Prop2, 'Prop23ad85376-ab00-4589-9328-27fa403c7cfc') == 0) OR Prop2 Contains 'Prop24ee'))
                        filterBuilder
                            .Where(model => model.Prop1, filter => filter.Prop1.In)
                            .And(model => model.Prop1, filter => filter.Prop1.NotIn)
                            .Or(spec3);
                    })
                    .ToList();

                actual.Should().BeEquivalentTo(expected);
            }
        }
    }
}
