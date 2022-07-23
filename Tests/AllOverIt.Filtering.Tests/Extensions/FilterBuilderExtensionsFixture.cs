using AllOverIt.Filtering.Builders;
using AllOverIt.Filtering.Builders.Extensions;
using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Options;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Filtering.Tests.Extensions
{
    public class FilterBuilderExtensionsFixture : FixtureBase
    {
        private class DummyClass
        {
            public int Prop1 { get; }
            public double Prop2 { get; }
        }

        private class DummyFilter
        {
            public class Prop1Filter
            {
                public EqualTo<int> EqualTo { get; set; } = new();
                public In<int> In { get; set; } = new();
            }

            public class Prop2Filter
            {
                public LessThanOrEqual<double> LessThanOrEqual { get; set; } = new();
                public GreaterThanOrEqual<double> GreaterThanOrEqual { get; set; } = new();
            }

            public Prop1Filter Prop1 { get; init; } = new();
            public Prop2Filter Prop2 { get; init; } = new();
        }

        [Fact]
        public void Should_Throw_When_Filter_Null()
        {
            Invoking(() =>
            {
                _ = FilterBuilderExtensions.ToQueryString((FilterBuilder<DummyClass, DummyFilter>) null);
            })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("filterBuilder");
        }

        [Fact]
        public void Should_Create_QueryString()
        {
            var filter = new DummyFilter
            {
                Prop1 =
                {
                    EqualTo = Create<int>(),
                    In = CreateMany<int>().ToList()
                },
                Prop2 =
                {
                    LessThanOrEqual = Create<double>(),
                    GreaterThanOrEqual =  Create<double>()
                }
            };

            var specificationBuilder = new FilterSpecificationBuilder<DummyClass, DummyFilter>(filter, A.Fake<IQueryFilterOptions>());
            var filterBuilder = new FilterBuilder<DummyClass, DummyFilter>(specificationBuilder);

            // When applied like this, they are chained from left to right. Use explicit specfications to control precedence.
            _ = filterBuilder
                .Where(model => model.Prop1, f => f.Prop1.EqualTo)                  // 1
                .Or(model => model.Prop1, f => f.Prop1.In)                          // 1 OR 2
                .And(model => model.Prop2, f => f.Prop2.LessThanOrEqual)            // (1 OR 2) AND 3
                .And(model => model.Prop2, f => f.Prop2.GreaterThanOrEqual);        // ((1 OR 2) AND 3) AND 4

            var queryString = filterBuilder.ToQueryString();

            var prop1EqualTo = filter.Prop1.EqualTo.Value;
            var prop1In = filter.Prop1.In.Value;
            var prop2LessThanOrEqual = filter.Prop2.LessThanOrEqual.Value;
            var prop2GreaterThanOrEqual = filter.Prop2.GreaterThanOrEqual.Value;

            // Example:
            // ((((Prop1 == 175) OR (109, 206, 12, 195, 17) Contains Prop1) AND (Prop2 <= 289.6046517882983)) AND (Prop2 >= 145.5696846337848))

            // Note: Binary operators result in surrounding ( ), while unary operators do not

            var step1 = $"(Prop1 == {prop1EqualTo})";                                   // Binary
            var step2 = $"({string.Join(", ", prop1In)}) Contains Prop1";               // Unary, but () surround the numbers
            var step3 = $"(Prop2 <= {prop2LessThanOrEqual})";                           // Binary
            var step4 = $"(Prop2 >= {prop2GreaterThanOrEqual})";                        // Binary

            // Resulting in: (((1 OR 2) AND 3) AND 4)
            var expected = $"((({step1} OR {step2}) AND {step3}) AND {step4})";

            queryString.Should().Be(expected);
        }
    }
}
