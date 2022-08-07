using AllOverIt.Expressions.Strings;
using AllOverIt.Filtering.Builders;
using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Options;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Filtering.Tests
{
    // Note: The tests for FilterSpecificationBuilder looks more at the different combinations
    //       of possible criteria whereas these tests focus on applying the options.
    public class FilterBuilderFixture : FixtureBase
    {
        private class DummyClass
        {
            public int Prop1 { get; set; }
            public double? Prop2 { get; set; }
            public string Prop3 { get; set; }
        }

        private class DummyFilter
        {
            public class Prop1Filter
            {
                public EqualTo<int> EqualTo { get; set; } = new();
                public NotEqualTo<int?> NotEqualTo { get; set; } = new();
                public In<int> In { get; set; } = new();
                public NotIn<int?> NotIn { get; set; } = new();
            }

            public class Prop2Filter
            {
                public EqualTo<double> EqualTo { get; set; } = new();
                public NotEqualTo<double?> NotEqualTo { get; set; } = new();
                public In<double> In { get; set; } = new();
                public NotIn<double?> NotIn { get; set; } = new();
            }

            public class Prop3Filter
            {
                public EqualTo<string> EqualTo { get; set; } = new();
                public In<string> In { get; set; } = new();
                public Contains Contains { get; set; } = new();
            }

            public Prop1Filter Prop1 { get; init; } = new();
            public Prop2Filter Prop2 { get; init; } = new();
            public Prop3Filter Prop3 { get; init; } = new();
        }

        public class Constructor : FilterBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_SpecificationBuilder_Null()
            {
                Invoking(() =>
                {
                    _ = new FilterBuilder<DummyClass, DummyFilter>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("specificationBuilder");
            }
        }

        private DummyFilter _filter;

        public FilterBuilderFixture()
        {
            _filter = new DummyFilter
            {
                Prop1 =
                {
                    EqualTo = Create<int>(),
                    NotEqualTo = Create<int?>(),
                    In = CreateMany<int>().ToList(),
                    NotIn = CreateMany<int?>().ToList()
                },
                Prop2 =
                {
                    EqualTo = Create<double>(),
                    NotEqualTo = Create<double?>(),
                    In = CreateMany<double>().ToList(),
                    NotIn = CreateMany<double?>().ToList()
                },
                Prop3 =
                {
                    EqualTo = Create<string>(),
                    In = CreateMany<string>().ToList(),
                    Contains = Create<string>()
                }
            };

        }

        public class Current : FilterBuilderFixture
        {
            [Fact]
            public void Should_Return_Self()
            {
                var filterBuilder = CreateFilterBuilder();

                var actual = filterBuilder.Current;

                actual.Should().BeSameAs(filterBuilder);
            }

            [Fact]
            public void Should_Return_As_ILogicalFilterBuilder()
            {
                var filterBuilder = CreateFilterBuilder();

                var actual = filterBuilder.Current;

                actual.Should().BeAssignableTo<ILogicalFilterBuilder<DummyClass, DummyFilter>>();
            }
        }

        public class AsSpecification : FilterBuilderFixture
        {
            [Fact]
            public void Should_Return_True_Specification_When_No_Filter()
            {
                var filterBuilder = CreateFilterBuilder();

                var actual = filterBuilder.AsSpecification();

                var expected = FilterSpecificationBuilder<DummyClass, DummyFilter>.SpecificationTrue;

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Specification()
            {
                var filterBuilder = CreateFilterBuilder();

                filterBuilder.Where(model => model.Prop1, filter => filter.Prop1.EqualTo);
                
                var actual = filterBuilder.AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.EqualTo.Value
                };

                actual.IsSatisfiedBy(model).Should().BeTrue();

                model.Prop1++;

                actual.IsSatisfiedBy(model).Should().BeFalse();
            }
        }

        public class Where_String : FilterBuilderFixture
        {
            [Theory]
            [MemberData(nameof(FilterComparisonOptions))]
            public void Should_Apply_Filter(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(FilterComparisonOptions))]
            public void Should_Ignore_Filter(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = true
                };

                // Start by confirming the filter is applied

                var filterBuilder1 = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder1
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop3 = Create<string>()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeFalse();

                // Now confirm the filter is ignored
                _filter.Prop3.EqualTo.Value = null;

                var filterBuilder2 = CreateFilterBuilder(_filter, options);

                specification = filterBuilder2
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .AsSpecification();

                actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }
        }

        public class Where_Basic : FilterBuilderFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_Filter(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop2, filter => filter.Prop2.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop2 = _filter.Prop2.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Filter(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                // Start by confirming the filter is applied

                var filterBuilder1 = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder1
                    .Where(model => model.Prop2, filter => filter.Prop2.In)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop2 = Create<double>()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeFalse();

                // Now confirm the filter is ignored
                _filter.Prop2.In.Value = null;

                var filterBuilder2 = CreateFilterBuilder(_filter, options);

                specification = filterBuilder2
                    .Where(model => model.Prop2, filter => filter.Prop2.In)
                    .AsSpecification();

                actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_Filters_Matching_Types(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                var filterBuilder1 = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder1
                    .Where(model => model.Prop1, filter => filter.Prop1.EqualTo)
                    .And(model => model.Prop2, filter => filter.Prop2.NotEqualTo)
                    .And(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.EqualTo.Value,
                    Prop2 = _filter.Prop2.NotEqualTo.Value + 1,
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();

                // and confirm
                model.Prop2 = _filter.Prop2.NotEqualTo.Value;

                actual = specification.IsSatisfiedBy(model);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_Filters_Different_Types(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                var filterBuilder1 = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder1
                    .Where(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .And(model => model.Prop2, filter => filter.Prop2.EqualTo)
                    .And(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.NotEqualTo.Value.Value + 1,
                    Prop2 = _filter.Prop2.EqualTo.Value,
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();

                // and confirm
                model.Prop2 = _filter.Prop2.NotEqualTo.Value;

                actual = specification.IsSatisfiedBy(model);

                actual.Should().BeFalse();
            }
        }

        public class Where_Array : FilterBuilderFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_Filter(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.In)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.In.Value[1]
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Filter(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                // Start by confirming the filter is applied

                var filterBuilder1 = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder1
                   .Where(model => model.Prop1, filter => filter.Prop1.In)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeFalse();

                // Now confirm the filter is ignored
                _filter.Prop1.In.Value = null;

                var filterBuilder2 = CreateFilterBuilder(_filter, options);

                specification = filterBuilder2
                    .Where(model => model.Prop1, filter => filter.Prop1.In)
                    .AsSpecification();

                actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_Filters_Matching_Types(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                var filterBuilder1 = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder1
                    .Where(model => model.Prop1, filter => filter.Prop1.EqualTo)
                    .And(model => model.Prop2, filter => filter.Prop2.NotEqualTo)
                    .And(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.EqualTo.Value,
                    Prop2 = _filter.Prop2.NotEqualTo.Value + 1,
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();

                // and confirm
                model.Prop2 = _filter.Prop2.NotEqualTo.Value;

                actual = specification.IsSatisfiedBy(model);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_Filters_Different_Types(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                var filterBuilder1 = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder1
                    .Where(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .And(model => model.Prop2, filter => filter.Prop2.EqualTo)
                    .And(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.NotEqualTo.Value.Value + 1,
                    Prop2 = _filter.Prop2.EqualTo.Value,
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();

                // and confirm
                model.Prop2 = _filter.Prop2.NotEqualTo.Value;

                actual = specification.IsSatisfiedBy(model);

                actual.Should().BeFalse();
            }
        }

        public class Where_And_String : FilterBuilderFixture
        {
            [Theory]
            [MemberData(nameof(FilterComparisonOptions))]
            public void Should_Apply_And_Filter(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .And(model => model.Prop1, filter => filter.Prop1.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.EqualTo.Value,
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(FilterComparisonOptions))]
            public void Should_Apply_Nullable_Filter(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .And(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.NotEqualTo.Value.Value + 1,
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(FilterComparisonOptions))]
            public void Should_Ignore_Nullable_Filter1(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop1.NotEqualTo.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .And(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>(),
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(FilterComparisonOptions))]
            public void Should_Ignore_Nullable_Filter2(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop3.EqualTo.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .And(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.NotEqualTo.Value.Value + 1,
                    Prop3 = Create<string>()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(FilterComparisonOptions))]
            public void Should_Ignore_Nullable_Filter3(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop1.NotEqualTo.Value = null;
                _filter.Prop3.EqualTo.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .And(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>(),
                    Prop3 = Create<string>()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();       // the builder has completely ignored both filters, so everything should return true
            }
        }

        public class Where_And_Basic : FilterBuilderFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_And_Filter(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.EqualTo)
                    .And(model => model.Prop2, filter => filter.Prop2.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.EqualTo.Value,
                    Prop2 = _filter.Prop2.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_Nullable_Filter(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .And(model => model.Prop2, filter => filter.Prop2.NotEqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.NotEqualTo.Value.Value + 1,
                    Prop2 = _filter.Prop2.NotEqualTo.Value.Value + 1
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Nullable_Filter1(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop2.NotEqualTo.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.EqualTo)
                    .And(model => model.Prop2, filter => filter.Prop2.NotEqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.EqualTo.Value,
                    Prop2 = Create<double>()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Nullable_Filter2(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop1.NotEqualTo.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .And(model => model.Prop2, filter => filter.Prop2.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>(),
                    Prop2 = _filter.Prop2.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Nullable_Filter3(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop1.NotEqualTo.Value = null;
                _filter.Prop2.NotEqualTo.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .And(model => model.Prop2, filter => filter.Prop2.NotEqualTo)
                    .And(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>(),
                    Prop2 = Create<double>(),
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();       // the builder has completely ignored both nullable filters, so everything should return true
            }
        }

        public class Where_And_Array : FilterBuilderFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_And_Filter(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.In)
                    .And(model => model.Prop2, filter => filter.Prop2.In)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.In.Value[1],
                    Prop2 = _filter.Prop2.In.Value[2]
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_Nullable_Filter(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.In)
                    .And(model => model.Prop2, filter => filter.Prop2.NotIn)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.In.Value[1],
                    Prop2 = _filter.Prop2.In.Value.Sum()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Nullable_Filter1(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop1.NotIn.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.NotIn)
                    .And(model => model.Prop2, filter => filter.Prop2.NotIn)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>(),
                    Prop2 = _filter.Prop2.NotIn.Value.Sum()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Nullable_Filter2(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop2.NotIn.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.NotIn)
                    .And(model => model.Prop2, filter => filter.Prop2.NotIn)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.NotIn.Value.Sum(item => item.Value),
                    Prop2 = Create<double>()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Nullable_Filter3(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop1.NotIn.Value = null;
                _filter.Prop2.NotIn.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.NotIn)
                    .And(model => model.Prop2, filter => filter.Prop2.NotIn)
                    .And(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>(),
                    Prop2 = Create<double>(),
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();       // the builder has completely ignored both nullable filters, so everything should return true
            }
        }

        public class Where_Or_String : FilterBuilderFixture
        {
            [Theory]
            [MemberData(nameof(FilterComparisonOptions))]
            public void Should_Apply_Or_Filter(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .Or(model => model.Prop1, filter => filter.Prop1.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.EqualTo.Value,
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(FilterComparisonOptions))]
            public void Should_Apply_Nullable_Filter(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .Or(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.NotEqualTo.Value.Value + 1,
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(FilterComparisonOptions))]
            public void Should_Ignore_Nullable_Filter1(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop1.NotEqualTo.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .Or(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>(),
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(FilterComparisonOptions))]
            public void Should_Ignore_Nullable_Filter2(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop3.EqualTo.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .Or(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.NotEqualTo.Value.Value + 1,
                    Prop3 = Create<string>()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(FilterComparisonOptions))]
            public void Should_Ignore_Nullable_Filter3(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop1.NotEqualTo.Value = null;
                _filter.Prop3.EqualTo.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .Or(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>(),
                    Prop3 = Create<string>()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();       // the builder has completely ignored both filters, so everything should return true
            }
        }

        public class Where_Or_Basic : FilterBuilderFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_Or_Filter(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.EqualTo)
                    .Or(model => model.Prop2, filter => filter.Prop2.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.EqualTo.Value,
                    Prop2 = _filter.Prop2.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_Nullable_Filter(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .Or(model => model.Prop2, filter => filter.Prop2.NotEqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.NotEqualTo.Value.Value + 1,
                    Prop2 = _filter.Prop2.NotEqualTo.Value.Value + 1
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Nullable_Filter1(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop2.NotEqualTo.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.EqualTo)
                    .Or(model => model.Prop2, filter => filter.Prop2.NotEqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.EqualTo.Value,
                    Prop2 = Create<double>()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Nullable_Filter2(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop1.NotEqualTo.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .Or(model => model.Prop2, filter => filter.Prop2.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>(),
                    Prop2 = _filter.Prop2.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Nullable_Filter3(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop1.NotEqualTo.Value = null;
                _filter.Prop2.NotEqualTo.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.NotEqualTo)
                    .Or(model => model.Prop2, filter => filter.Prop2.NotEqualTo)
                    .Or(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>(),
                    Prop2 = Create<double>(),
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();       // the builder has completely ignored both nullable filters, so everything should return true
            }
        }

        public class Where_Or_Array : FilterBuilderFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_Or_Filter(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.In)
                    .Or(model => model.Prop2, filter => filter.Prop2.In)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.In.Value[1],
                    Prop2 = _filter.Prop2.In.Value[2]
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_Nullable_Filter(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.In)
                    .Or(model => model.Prop2, filter => filter.Prop2.NotIn)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.In.Value[1],
                    Prop2 = _filter.Prop2.In.Value.Sum()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Nullable_Filter1(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop1.NotIn.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.NotIn)
                    .Or(model => model.Prop2, filter => filter.Prop2.NotIn)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>(),
                    Prop2 = _filter.Prop2.NotIn.Value.Sum()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Nullable_Filter2(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop2.NotIn.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.NotIn)
                    .Or(model => model.Prop2, filter => filter.Prop2.NotIn)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = _filter.Prop1.NotIn.Value.Sum(item => item.Value),
                    Prop2 = Create<double>()
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Ignore_Nullable_Filter3(bool useParameterizedQueries)
            {
                var options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = true
                };

                _filter.Prop1.NotIn.Value = null;
                _filter.Prop2.NotIn.Value = null;

                var filterBuilder = CreateFilterBuilder(_filter, options);

                var specification = filterBuilder
                    .Where(model => model.Prop1, filter => filter.Prop1.NotIn)
                    .Or(model => model.Prop2, filter => filter.Prop2.NotIn)
                    .Or(model => model.Prop3, filter => filter.Prop3.EqualTo)
                    .AsSpecification();

                var model = new DummyClass
                {
                    Prop1 = Create<int>(),
                    Prop2 = Create<double>(),
                    Prop3 = _filter.Prop3.EqualTo.Value
                };

                var actual = specification.IsSatisfiedBy(model);

                actual.Should().BeTrue();       // the builder has completely ignored both nullable filters, so everything should return true
            }
        }

        private IFilterBuilder<DummyClass, DummyFilter> CreateFilterBuilder(DummyFilter filter = null, DefaultQueryFilterOptions options = null)
        {
            filter ??= _filter;
            options ??= new DefaultQueryFilterOptions();

            var specificationBuilder = new FilterSpecificationBuilder<DummyClass, DummyFilter>(filter, options);
            return new FilterBuilder<DummyClass, DummyFilter>(specificationBuilder);
        }

        protected static IEnumerable<object[]> FilterComparisonOptions()
        {
            return new List<object[]>
            {
                new object[] { false, StringComparisonMode.None },
                new object[] { true, StringComparisonMode.None },
                new object[] { false, StringComparisonMode.ToUpper },
                new object[] { true, StringComparisonMode.ToUpper  },
                new object[] { false, StringComparisonMode.ToLower },
                new object[] { true, StringComparisonMode.ToLower },
                new object[] { false, StringComparisonMode.InvariantCultureIgnoreCase },
                new object[] { true, StringComparisonMode.InvariantCultureIgnoreCase },
            };
        }
    }           
}
