using AllOverIt.Filtering.Extensions;
using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Filtering.Tests.Extensions
{
    public class FilterExtensionsFixture : FixtureBase
    {
        private class DummyFilter
        {
            public class Prop1Filter
            {
                public EqualTo<int?> EqualTo { get; set; } = new();
                public In<int?> In { get; set; } = new();
            }

            public class Prop2Filter
            {
                public StartsWith StartsWith { get; set; } = new();
                public EqualTo<string> EqualTo { get; set; } = new();
                public In<string> In { get; set; } = new();
            }

            public Prop1Filter Prop1 { get; init; } = new();
            public Prop2Filter Prop2 { get; init; } = new();
        }

        public class HasValue_Array : FilterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    _ = FilterExtensions.HasValue((IArrayFilterOperation<int>)default);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("filter");
            }

            [Fact]
            public void Should_Return_False_When_Value_Null()
            {
                var filter = new DummyFilter();

                filter.Prop1.In.Value.Should().BeNull();
                filter.Prop2.In.Value.Should().BeNull();

                var actual1 = FilterExtensions.HasValue(filter.Prop1.In);
                var actual2 = FilterExtensions.HasValue(filter.Prop2.In);

                actual1.Should().BeFalse();
                actual2.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_False_When_Value_Empty()
            {
                var filter = new DummyFilter
                {
                    Prop1 =
                    {
                        In = new List<int?>()
                    },
                    Prop2 =
                    {
                        In = new List<string>()
                    }
                };

                filter.Prop1.In.Value.Should().BeEmpty();
                filter.Prop2.In.Value.Should().BeEmpty();

                var actual1 = FilterExtensions.HasValue(filter.Prop1.In);
                var actual2 = FilterExtensions.HasValue(filter.Prop2.In);

                actual1.Should().BeFalse();
                actual2.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_Value_Not_Empty()
            {
                var filter = new DummyFilter
                {
                    Prop1 =
                    {
                        In = new List<int?>{ 1, 2, 3 }
                    },
                    Prop2 =
                    {
                        In = new List<string>{ "" }     // an empty string is still an element
                    }
                };

                filter.Prop1.In.Value.Should().NotBeEmpty();
                filter.Prop2.In.Value.Should().NotBeEmpty();

                var actual1 = FilterExtensions.HasValue(filter.Prop1.In);
                var actual2 = FilterExtensions.HasValue(filter.Prop2.In);

                actual1.Should().BeTrue();
                actual2.Should().BeTrue();
            }
        }

        public class HasValue_String : FilterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    _ = FilterExtensions.HasValue((IStringFilterOperation) default);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("filter");
            }

            [Fact]
            public void Should_Return_False_When_Value_Null()
            {
                var filter = new DummyFilter();

                filter.Prop2.StartsWith.Value.Should().BeNull();

                var actual = FilterExtensions.HasValue(filter.Prop2.StartsWith);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_Value_Empty()
            {
                var filter = new DummyFilter
                {
                    Prop2 =
                    {
                        StartsWith = string.Empty
                    }
                };

                filter.Prop2.StartsWith.Value.Should().BeEmpty();

                var actual = FilterExtensions.HasValue(filter.Prop2.StartsWith);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_True_When_Value_Whitespace()
            {
                var filter = new DummyFilter
                {
                    Prop2 =
                    {
                        StartsWith = "  "
                    }
                };

                var actual = FilterExtensions.HasValue(filter.Prop2.StartsWith);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_True_When_Value_Not_Empty()
            {
                var filter = new DummyFilter
                {
                    Prop2 =
                    {
                        StartsWith = Create<string>()
                    }
                };

                var actual = FilterExtensions.HasValue(filter.Prop2.StartsWith);

                actual.Should().BeTrue();
            }
        }

        public class Any_Basic : FilterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    _ = FilterExtensions.HasValue((IBasicFilterOperation<int?>) default);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("filter");
            }

            [Fact]
            public void Should_Return_False_When_Value_Null()
            {
                var filter = new DummyFilter();

                filter.Prop1.EqualTo.Value.Should().BeNull();

                var actual = FilterExtensions.HasValue(filter.Prop1.EqualTo);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_Value_Not_Empty()
            {
                var filter = new DummyFilter
                {
                    Prop1 =
                    {
                        EqualTo = Create<int>()
                    }
                };

                var actual = FilterExtensions.HasValue(filter.Prop1.EqualTo);

                actual.Should().BeTrue();
            }
        }
    }
}
