using AllOverIt.Expressions.Strings;
using AllOverIt.Filtering.Builders;
using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Options;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Specification;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Equivalency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Filtering.Tests
{
    public class FilterSpecificationBuilderFixture : FixtureBase
    {
        public enum DummyEntityCategory
        {
            Furniture,
            Clothing,
            Stationary
        }

        private sealed class DummyEntity
        {
            public bool Active { get; set; }
            public DummyEntityCategory Category { get; set; }
            public string Name { get; set; }
            public double? Price { get; set; }
            public DateTime? LastUpdated { get; set; }
        }

        public sealed class DummyEntityFilter
        {
            public sealed class ActiveFilter
            {
                public EqualTo<bool?> EqualTo { get; set; } = new();
            }

            public sealed class CategoryFilter
            {
                public EqualTo<DummyEntityCategory> EqualTo { get; set; } = new();
                public NotEqualTo<DummyEntityCategory?> NotEqualTo { get; set; } = new();
            }

            public sealed class NameFilter
            {
                public Contains Contains { get; set; } = new();
                public NotContains NotContains { get; set; } = new();
                public StartsWith StartsWith { get; set; } = new();
                public EndsWith EndsWith { get; set; } = new();
                public In<string> In { get; set; } = new();
                public NotIn<string> NotIn { get; set; } = new();
                public GreaterThan<string> GreaterThan { get; set; } = new();
                public GreaterThanOrEqual<string> GreaterThanOrEqual { get; set; } = new();
                public LessThan<string> LessThan { get; set; } = new();
                public LessThanOrEqual<string> LessThanOrEqual { get; set; } = new();
                public EqualTo<string> EqualTo { get; set; } = new();
                public NotEqualTo<string> NotEqualTo { get; set; } = new();
            }

            public sealed class PriceFilter
            {
                public In<double?> In { get; set; } = new();
                public NotIn<double> NotIn { get; set; } = new();
                public GreaterThan<double> GreaterThan { get; set; } = new();
                public GreaterThanOrEqual<double> GreaterThanOrEqual { get; set; } = new();
                public LessThan<double> LessThan { get; set; } = new();
                public LessThanOrEqual<double> LessThanOrEqual { get; set; } = new();
                public EqualTo<double> EqualTo { get; set; } = new();
                public NotEqualTo<double> NotEqualTo { get; set; } = new();
            }

            public sealed class LastUpdatedFilter
            {
                public LessThanOrEqual<DateTime> LessThanOrEqual { get; set; } = new();
                public GreaterThanOrEqual<DateTime?> GreaterThanOrEqual { get; set; } = new();
            }

            public ActiveFilter Active { get; init; } = new();
            public CategoryFilter Category { get; init; } = new();
            public NameFilter Name { get; init; } = new();
            public PriceFilter Price { get; init; } = new();
            public LastUpdatedFilter LastUpdated { get; init; } = new();
        }

        private DummyEntityFilter _filter;
        private IDefaultQueryFilterOptions _options;
        private IFilterSpecificationBuilder<DummyEntity, DummyEntityFilter> _specificationBuilder;

        public FilterSpecificationBuilderFixture()
        {
            _filter = new DummyEntityFilter
            {
                //Active =
                //{
                //    EqualTo = Create<bool>()
                //},
                Category =
                {
                    EqualTo = Create<DummyEntityCategory>(),
                    NotEqualTo = Create<DummyEntityCategory>()
                },
                Name =
                {
                    Contains = Create<string>(),
                    NotContains = Create<string>(),
                    StartsWith = Create<string>(),
                    EndsWith = Create<string>(),
                    In = new In<string>(CreateMany<string>().ToArray()),        // A specific example using ToArray() that previously failed a test
                    NotIn = CreateMany<string>().ToList(),
                    GreaterThan = Create<string>(),
                    GreaterThanOrEqual = Create<string>(),
                    LessThan = Create<string>(),
                    LessThanOrEqual = Create<string>(),
                    EqualTo = Create<string>(),
                    NotEqualTo = Create<string>()
                },
                Price =
                {
                    In = new In<double?>(CreateMany<double?>().ToArray()),      // Testing ToArray with double?
                    NotIn = CreateMany<double>().ToList(),
                    GreaterThan = Create<double>(),
                    GreaterThanOrEqual = Create<double>(),
                    LessThan = Create<double>(),
                    LessThanOrEqual = Create<double>(),
                    EqualTo = Create<double>(),
                    NotEqualTo = Create<double>()
                },
                LastUpdated =
                {
                    LessThanOrEqual = DateTime.UtcNow,
                    GreaterThanOrEqual = DateTime.UtcNow.AddDays(-7)
                }
            };

            _options = new DefaultQueryFilterOptions
            {
                UseParameterizedQueries = Create<bool>(),
                StringComparisonMode = Create<bool>() ? StringComparisonMode.None : StringComparisonMode.ToLower,
                IgnoreNullFilterValues = false
            };

            _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);
        }

        public class Constructor : FilterSpecificationBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Filter_Null()
            {
                Invoking(() =>
                {
                    _ = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(null, A.Fake<IDefaultQueryFilterOptions>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("filter");
            }

            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    _ = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(Create<DummyEntityFilter>(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("options");
            }
        }

        public class Create_StringFilter : FilterSpecificationBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyExpression_Null()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Create((Expression<Func<DummyEntity, string>>)null, f => f.Name.Contains);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Operation_Null()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Create(model => model.Name, (Func<DummyEntityFilter, IStringFilterOperation>)null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation");
            }

            [Fact]
            public void Should_Not_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Create(model => model.Name, f => f.Name.Contains, null);
                })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_Contains(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.Contains);

                var entityName = $"{Create<string>()}{_filter.Name.Contains.Value}{Create<string>()}";

                AssertNameSpecification(specification, entityName, Create<string>());
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_NotContains(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.NotContains);

                var entityName = $"{Create<string>()}{_filter.Name.NotContains.Value}{Create<string>()}";

                AssertNameSpecification(specification, Create<string>(), entityName);
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_StartsWith(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.StartsWith);

                var entityName = $"{_filter.Name.StartsWith.Value}{Create<string>()}";

                AssertNameSpecification(specification, entityName, $"{Create<string>()}{_filter.Name.StartsWith.Value}");
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_EndsWith(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.EndsWith);

                var entityName = $"{Create<string>()}{_filter.Name.EndsWith.Value}";

                AssertNameSpecification(specification, entityName, $"{_filter.Name.EndsWith.Value}{Create<string>()}");
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_In(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.In);

                var entityName = _filter.Name.In.Value[2];

                AssertNameSpecification(specification, entityName, Create<string>());
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_NotIn(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.NotIn);

                var entityName = _filter.Name.NotIn.Value[2];

                AssertNameSpecification(specification, Create<string>(), entityName);
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_GreaterThan(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.GreaterThan);

                var entityName = $"{_filter.Name.GreaterThan.Value}ZZZ";

                AssertNameSpecification(specification, entityName, _filter.Name.GreaterThan.Value);
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_GreaterThanOrEqual(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.GreaterThanOrEqual);

                var entityName = $"{_filter.Name.GreaterThanOrEqual.Value}ZZZ";

                AssertNameSpecification(specification, entityName, _filter.Name.GreaterThanOrEqual.Value[..4]);
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_GreaterThanOrEqual_When_Equal(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.GreaterThanOrEqual);

                var entityName = _filter.Name.GreaterThanOrEqual.Value;

                AssertNameSpecification(specification, entityName, _filter.Name.GreaterThanOrEqual.Value[..4]);
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_LessThan(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.LessThan);

                var entityName = _filter.Name.LessThan.Value[..4];

                AssertNameSpecification(specification, entityName, _filter.Name.LessThan.Value);
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_LessThanOrEqual(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.LessThanOrEqual);

                var entityName = _filter.Name.LessThanOrEqual.Value[..4];

                AssertNameSpecification(specification, entityName, $"{_filter.Name.LessThanOrEqual.Value}ZZZ");
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_LessThanOrEqual_When_Equal(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.LessThanOrEqual);

                var entityName = _filter.Name.LessThanOrEqual.Value;

                AssertNameSpecification(specification, entityName, $"{_filter.Name.LessThanOrEqual.Value}ZZZ");
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_EqualTo(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.EqualTo);

                var entityName = _filter.Name.EqualTo.Value;

                AssertNameSpecification(specification, entityName, Create<string>());
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Create_NotEqualTo(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.NotEqualTo);

                var entityName = _filter.Name.NotEqualTo.Value;

                AssertNameSpecification(specification, Create<string>(), entityName);
            }

            [Fact]
            public void Should_Ignore_Null_Filter()
            {
                _options = new DefaultQueryFilterOptions
                {
                    IgnoreNullFilterValues = true
                };

                _filter.Name.EqualTo.Value = null;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.EqualTo);

                var entity = new DummyEntity
                {
                    Name = Create<string>()
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();
            }
        }

        public class Create_Value : FilterSpecificationBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyExpression_Null()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Create((Expression<Func<DummyEntity, double>>)null, f => f.Price.GreaterThan);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Operation_Null()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Create(model => model.Price, (Func<DummyEntityFilter, IBasicFilterOperation>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation");
            }

            [Fact]
            public void Should_Not_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Create(model => model.Price, f => f.Price.GreaterThan, null);
                })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Create_In(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Price, filter => filter.Price.In);

                var entityPrice = _filter.Price.In.Value[2];

                // As a test, deliberately passing double[] for a specification of type double?[]
                AssertPriceSpecification(specification, entityPrice, _filter.Price.In.Value.Select(item => item.Value).Sum());
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Create_NotIn(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Price, filter => filter.Price.NotIn);

                var entityPrice = _filter.Price.NotIn.Value[2];

                AssertPriceSpecification(specification, _filter.Price.In.Value.Sum(), entityPrice);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Create_GreaterThan(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Price, filter => filter.Price.GreaterThan);

                AssertPriceSpecification(specification, _filter.Price.GreaterThan.Value + 1, null);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Create_GreaterThanOrEqual(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Price, filter => filter.Price.GreaterThanOrEqual);

                AssertPriceSpecification(specification, _filter.Price.GreaterThanOrEqual.Value + 1, null);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Create_GreaterThanOrEqual_When_Equal(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Price, filter => filter.Price.GreaterThanOrEqual);

                AssertPriceSpecification(specification, _filter.Price.GreaterThanOrEqual.Value, null);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Create_LessThan(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Price, filter => filter.Price.LessThan);

                AssertPriceSpecification(specification, _filter.Price.LessThan.Value - 1, _filter.Price.LessThan.Value + 1);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Create_LessThanOrEqual(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Price, filter => filter.Price.LessThanOrEqual);

                AssertPriceSpecification(specification, _filter.Price.LessThanOrEqual.Value - 1, _filter.Price.LessThanOrEqual.Value + 1);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Create_LessThanOrEqual_When_Equal(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Price, filter => filter.Price.LessThanOrEqual);

                var entityPrice = _filter.Price.LessThanOrEqual.Value;

                AssertPriceSpecification(specification, _filter.Price.LessThanOrEqual.Value, _filter.Price.LessThanOrEqual.Value + 1);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Create_EqualTo(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Price, filter => filter.Price.EqualTo);

                AssertPriceSpecification(specification, _filter.Price.EqualTo.Value, _filter.Price.EqualTo.Value + 1);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Create_NotEqualTo(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Price, filter => filter.Price.NotEqualTo);

                AssertPriceSpecification(specification, _filter.Price.NotEqualTo.Value - 1, _filter.Price.NotEqualTo.Value);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Create_Support_Enum(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Category, filter => filter.Category.EqualTo);

                var entity = new DummyEntity
                {
                    Category = _filter.Category.EqualTo.Value
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();

                entity.Category = CreateExcluding(_filter.Category.EqualTo.Value);

                specification.IsSatisfiedBy(entity).Should().BeFalse();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Create_Support_Nullable_Enum(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Category, filter => filter.Category.NotEqualTo);

                var entity = new DummyEntity
                {
                    Category = CreateExcluding(_filter.Category.NotEqualTo.Value.Value)
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();

                entity.Category = _filter.Category.NotEqualTo.Value.Value;

                specification.IsSatisfiedBy(entity).Should().BeFalse();
            }

            [Fact]
            public void Should_Ignore_Null_Filter()
            {
                _options = new DefaultQueryFilterOptions
                {
                    IgnoreNullFilterValues = true
                };

                _filter.Active.EqualTo.Value = null;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Active, filter => filter.Active.EqualTo);

                var entities = new[]
                {
                    new DummyEntity
                    {
                        Active = false      // An ignored specification should still return this as found
                    },
                    new DummyEntity
                    {
                        Active = true
                    }
                };

                specification.IsSatisfiedBy(entities[0]).Should().BeTrue();
                specification.IsSatisfiedBy(entities[1]).Should().BeTrue();
            }
        }

        public class And_Mixed_BasicFilter_StringFilter : FilterSpecificationBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyExpression_Null_Basic_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And((Expression<Func<DummyEntity, string>>) null, f => f.Name.GreaterThan, f => f.Name.Contains);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Operation1_Null_Basic_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And(model => model.Name, (Func<DummyEntityFilter, IBasicFilterOperation<string>>) null, f => f.Name.Contains);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation1");
            }

            [Fact]
            public void Should_Throw_When_Operation2_Null_Basic_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And(model => model.Name, f => f.Name.GreaterThan, (Func<DummyEntityFilter, IStringFilterOperation>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation2");
            }

            [Fact]
            public void Should_Not_Throw_When_Options_Null_Basic_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And(model => model.Name, f => f.Name.GreaterThan, f => f.Name.Contains, null);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_PropertyExpression_Null_String_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And((Expression<Func<DummyEntity, string>>) null, f => f.Name.Contains, f => f.Name.GreaterThan);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Operation1_Null_String_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And(model => model.Name, (Func<DummyEntityFilter, IStringFilterOperation>) null, f => f.Name.GreaterThan);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation1");
            }

            [Fact]
            public void Should_Throw_When_Operation2_Null_String_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And(model => model.Name, f => f.Name.Contains, (Func<DummyEntityFilter, IBasicFilterOperation<string>>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation2");
            }

            [Fact]
            public void Should_Not_Throw_When_Options_Null_String_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And(model => model.Name, f => f.Name.Contains, f => f.Name.GreaterThan, null);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_PropertyExpression_Null_String_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And((Expression<Func<DummyEntity, string>>) null, f => f.Name.Contains, f => f.Name.NotContains);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Operation1_Null_String_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And(model => model.Name, (Func<DummyEntityFilter, IStringFilterOperation>) null, f => f.Name.NotContains);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation1");
            }

            [Fact]
            public void Should_Throw_When_Operation2_Null_String_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And(model => model.Name, f => f.Name.Contains, (Func<DummyEntityFilter, IStringFilterOperation>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation2");
            }

            [Fact]
            public void Should_Not_Throw_When_Options_Null_String_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And(model => model.Name, f => f.Name.Contains, f => f.Name.NotContains, null);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_PropertyExpression_Null_Basic_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And((Expression<Func<DummyEntity, double>>) null, f => f.Price.GreaterThan, f => f.Price.LessThan);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Operation1_Null_Basic_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And(model => model.Price, (Func<DummyEntityFilter, IBasicFilterOperation>) null, f => f.Price.GreaterThan);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation1");
            }

            [Fact]
            public void Should_Throw_When_Operation2_Null_Basic_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And(model => model.Price, f => f.Price.GreaterThan, (Func<DummyEntityFilter, IBasicFilterOperation>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation2");
            }

            [Fact]
            public void Should_Not_Throw_When_Options_Null_Basic_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.And(model => model.Price, f => f.Price.LessThan, f => f.Price.GreaterThan, null);
                })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(false, "AbC", "fgh", 0, "pqrAbCxyz", "xyz_abc_fgh")]    // StringComparisonMode.None
            [InlineData(true, "AbC", "fgh", 0, "pqrAbCxyz", "xyz_abc_fgh")]
            [InlineData(false, "AbC", "fgh", 1, "pqrAbCxyz", "cde")]            // StringComparisonMode.ToLower
            [InlineData(true, "AbC", "fgh", 1, "pqrAbCxyz", "cde")]
            [InlineData(false, "AbC", "fgh", 2, "pqrAbCxyz", "cde")]            // StringComparisonMode.ToUpper
            [InlineData(true, "AbC", "fgh", 2, "pqrAbCxyz", "cde")]
            [InlineData(false, "AbC", "fgh", 6, "pqrAbCxyz", "cde")]            // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, "AbC", "fgh", 6, "pqrAbCxyz", "cde")]
            public void Should_And_Contains_GreaterThan(bool useParameterizedQueries, string contains, string greaterThan,
                int mode, string trueValue, string falseValue)
{
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _filter.Name.Contains = contains;
                _filter.Name.GreaterThan = greaterThan;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.And(entity => entity.Name, filter => filter.Name.Contains, filter => filter.Name.GreaterThan);

                AssertNameSpecification(specification, trueValue, falseValue);
            }

            [Theory]
            [InlineData(false, "AbC", "fgh", 0, "pqrAbCxyz", "xyz_abc_fgh")]    // StringComparisonMode.None
            [InlineData(true, "AbC", "fgh", 0, "pqrAbCxyz", "xyz_abc_fgh")]
            [InlineData(false, "AbC", "fgh", 1, "pqrAbCxyz", "cde")]            // StringComparisonMode.ToLower
            [InlineData(true, "AbC", "fgh", 1, "pqrAbCxyz", "cde")]
            [InlineData(false, "AbC", "fgh", 2, "pqrAbCxyz", "cde")]            // StringComparisonMode.ToUpper
            [InlineData(true, "AbC", "fgh", 2, "pqrAbCxyz", "cde")]
            [InlineData(false, "AbC", "fgh", 6, "pqrAbCxyz", "cde")]            // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, "AbC", "fgh", 6, "pqrAbCxyz", "cde")]
            public void Should_And_GreaterThan_Contains(bool useParameterizedQueries, string contains, string greaterThan,
                int mode, string trueValue, string falseValue)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _filter.Name.Contains = contains;
                _filter.Name.GreaterThan = greaterThan;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.And(entity => entity.Name, filter => filter.Name.GreaterThan, filter => filter.Name.Contains);

                AssertNameSpecification(specification, trueValue, falseValue);
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_And_EqualTo_In(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                var newValues = _filter.Name.In.Value.Concat(new[] { _filter.Name.EqualTo.Value });     // It's array (fixed size), so cannot just add the value
                _filter.Name.In = new In<string>(newValues);

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.And(entity => entity.Name, filter => filter.Name.EqualTo, filter => filter.Name.In);

                var trueValue = _filter.Name.EqualTo.Value;
                var falseValue = Create<string>();

                AssertNameSpecification(specification, trueValue, falseValue);
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_And_NotIn_NotEqualTo(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.And(entity => entity.Name, filter => filter.Name.NotIn, filter => filter.Name.NotEqualTo);

                var trueValue = Create<string>();
                var falseValue = _filter.Name.NotEqualTo.Value;

                AssertNameSpecification(specification, trueValue, falseValue);
            }

            [Fact]
            public void Should_Ignore_Null_Filter()
            {
                _options = new DefaultQueryFilterOptions
                {
                    IgnoreNullFilterValues = true
                };

                _filter.Name.EqualTo.Value = null;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.And(entity => entity.Name, filter => filter.Name.EqualTo, filter => filter.Name.StartsWith);

                var entity = new DummyEntity
                {
                    Name = $"{_filter.Name.StartsWith.Value}ZZZ"
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();
            }
        }

        public class And_StringFilter : FilterSpecificationBuilderFixture
        {
            [Theory]
            [InlineData(false, "AbC", "xyz", 0, "xyz_AbC_fgh", "pqr")]      // StringComparisonMode.None
            [InlineData(true, "AbC", "xyz", 0, "xyz_AbC_fgh", "pqr")]
            [InlineData(false, "AbC", "xyz", 1, "xyz_AbC_fgh", "pqr")]      // StringComparisonMode.ToLower
            [InlineData(true, "AbC", "xyz", 1, "xyz_AbC_fgh", "pqr")]
            [InlineData(false, "AbC", "xyz", 2, "xyz_AbC_fgh", "pqr")]      // StringComparisonMode.ToUpper
            [InlineData(true, "AbC", "xyz", 2, "xyz_AbC_fgh", "pqr")]
            [InlineData(false, "AbC", "xyz", 6, "xyz_AbC_fgh", "pqr")]      // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, "AbC", "xyz", 6, "xyz_AbC_fgh", "pqr")]
            public void Should_And_Contains_StartsWith(bool useParameterizedQueries, string contains, string startsWith,
                int mode, string trueValue, string falseValue)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _filter.Name.Contains = contains;
                _filter.Name.StartsWith = startsWith;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.And(entity => entity.Name, filter => filter.Name.Contains, filter => filter.Name.StartsWith);

                AssertNameSpecification(specification, trueValue, falseValue);
            }

            [Theory]
            [InlineData(false, "AbC", "xyz", 0, "pqrxyz", "xyz_AbC_fgh")]      // StringComparisonMode.None
            [InlineData(true, "AbC", "xyz", 0, "pqrxyz", "xyz_AbC_fgh")]
            [InlineData(false, "AbC", "xyz", 1, "pqrxyz", "xyz_AbC_fgh")]      // StringComparisonMode.ToLower
            [InlineData(true, "AbC", "xyz", 1, "pqrxyz", "xyz_AbC_fgh")]
            [InlineData(false, "AbC", "xyz", 2, "pqrxyz", "xyz_AbC_fgh")]      // StringComparisonMode.ToUpper
            [InlineData(true, "AbC", "xyz", 2, "pqrxyz", "xyz_AbC_fgh")]
            [InlineData(false, "AbC", "xyz", 6, "pqrxyz", "xyz_AbC_fgh")]      // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, "AbC", "xyz", 6, "pqrxyz", "xyz_AbC_fgh")]
            public void Should_And_NotContains_EndsWith(bool useParameterizedQueries, string notContains, string endsWith,
               int mode, string trueValue, string falseValue)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _filter.Name.NotContains = notContains;
                _filter.Name.EndsWith = endsWith;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.And(entity => entity.Name, filter => filter.Name.NotContains, filter => filter.Name.EndsWith);

                AssertNameSpecification(specification, trueValue, falseValue);
            }

            [Fact]
            public void Should_Ignore_Null_Filter()
            {
                _options = new DefaultQueryFilterOptions
                {
                    IgnoreNullFilterValues = true
                };

                _filter.Name.NotContains.Value = null;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.And(entity => entity.Name, filter => filter.Name.NotContains, filter => filter.Name.StartsWith);

                var entity = new DummyEntity
                {
                    Name = $"{_filter.Name.StartsWith.Value}ZZZ"
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();
            }
        }

        public class And_Value : FilterSpecificationBuilderFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_And_LessThanOrEqual_GreaterThanOrEqual(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var lastUpdated = DateTime.Now.Date;

                _filter.LastUpdated.LessThanOrEqual = lastUpdated.AddDays(1);
                _filter.LastUpdated.GreaterThanOrEqual = lastUpdated.AddDays(-1);

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.And(
                    entity => entity.LastUpdated,
                    filter => filter.LastUpdated.LessThanOrEqual,
                    filter => filter.LastUpdated.GreaterThanOrEqual);

                var entityPrice = _filter.Price.In.Value[2];

                AssertLastUpdatedSpecification(specification, lastUpdated, DateTime.MinValue);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_And_EqualTo_GreaterThanOrEqual(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var entityPrice = Create<double>();

                _filter.Price.EqualTo = entityPrice;
                _filter.Price.GreaterThanOrEqual = entityPrice - 1;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.And(
                    entity => entity.Price,
                    filter => filter.Price.EqualTo,
                    filter => filter.Price.GreaterThanOrEqual);

                AssertPriceSpecification(specification, entityPrice, -entityPrice);
            }

            [Fact]
            public void Should_Ignore_Null_Filter()
            {
                _options = new DefaultQueryFilterOptions
                {
                    IgnoreNullFilterValues = true
                };

                _filter.LastUpdated.GreaterThanOrEqual.Value = null;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.And(entity => entity.LastUpdated, filter => filter.LastUpdated.LessThanOrEqual, filter => filter.LastUpdated.GreaterThanOrEqual);

                var entity = new DummyEntity
                {
                    LastUpdated = _filter.LastUpdated.LessThanOrEqual.Value.AddDays(-1)
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();
            }
        }

        public class Or_Mixed_BasicFilter_StringFilter : FilterSpecificationBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyExpression_Null_Basic_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or((Expression<Func<DummyEntity, string>>) null, f => f.Name.GreaterThan, f => f.Name.Contains);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Operation1_Null_Basic_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or(model => model.Name, (Func<DummyEntityFilter, IBasicFilterOperation<string>>) null, f => f.Name.Contains);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation1");
            }

            [Fact]
            public void Should_Throw_When_Operation2_Null_Basic_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or(model => model.Name, f => f.Name.GreaterThan, (Func<DummyEntityFilter, IStringFilterOperation>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation2");
            }

            [Fact]
            public void Should_Not_Throw_When_Options_Null_Basic_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or(model => model.Name, f => f.Name.GreaterThan, f => f.Name.Contains, null);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_PropertyExpression_Null_String_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or((Expression<Func<DummyEntity, string>>) null, f => f.Name.Contains, f => f.Name.GreaterThan);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Operation1_Null_String_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or(model => model.Name, (Func<DummyEntityFilter, IStringFilterOperation>) null, f => f.Name.GreaterThan);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation1");
            }

            [Fact]
            public void Should_Throw_When_Operation2_Null_String_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or(model => model.Name, f => f.Name.Contains, (Func<DummyEntityFilter, IBasicFilterOperation<string>>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation2");
            }

            [Fact]
            public void Should_Not_Throw_When_Options_Null_String_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or(model => model.Name, f => f.Name.Contains, f => f.Name.GreaterThan, null);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_PropertyExpression_Null_String_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or((Expression<Func<DummyEntity, string>>) null, f => f.Name.Contains, f => f.Name.NotContains);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Operation1_Null_String_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or(model => model.Name, (Func<DummyEntityFilter, IStringFilterOperation>) null, f => f.Name.NotContains);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation1");
            }

            [Fact]
            public void Should_Throw_When_Operation2_Null_String_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or(model => model.Name, f => f.Name.Contains, (Func<DummyEntityFilter, IStringFilterOperation>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation2");
            }

            [Fact]
            public void Should_Not_Throw_When_Options_Null_String_String()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or(model => model.Name, f => f.Name.Contains, f => f.Name.NotContains, null);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_PropertyExpression_Null_Basic_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or((Expression<Func<DummyEntity, double>>) null, f => f.Price.GreaterThan, f => f.Price.LessThan);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Operation1_Null_Basic_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or(model => model.Price, (Func<DummyEntityFilter, IBasicFilterOperation>) null, f => f.Price.GreaterThan);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation1");
            }

            [Fact]
            public void Should_Throw_When_Operation2_Null_Basic_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or(model => model.Price, f => f.Price.GreaterThan, (Func<DummyEntityFilter, IBasicFilterOperation>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation2");
            }

            [Fact]
            public void Should_Not_Throw_When_Options_Null_Basic_Basic()
            {
                Invoking(() =>
                {
                    _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                    _specificationBuilder.Or(model => model.Price, f => f.Price.LessThan, f => f.Price.GreaterThan, null);
                })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(false, "AbC", "fgh", 0, "pqrAbCxyz", "cde")]      // StringComparisonMode.None
            [InlineData(true, "AbC", "fgh", 0, "pqrAbCxyz", "cde")]
            [InlineData(false, "AbC", "fgh", 1, "pqrAbCxyz", "cde")]      // StringComparisonMode.ToLower
            [InlineData(true, "AbC", "fgh", 1, "pqrAbCxyz", "cde")]
            [InlineData(false, "AbC", "fgh", 2, "pqrAbCxyz", "cde")]      // StringComparisonMode.ToUpper
            [InlineData(true, "AbC", "fgh", 2, "pqrAbCxyz", "cde")]
            [InlineData(false, "AbC", "fgh", 6, "pqrAbCxyz", "cde")]      // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, "AbC", "fgh", 6, "pqrAbCxyz", "cde")]
            public void Should_Or_Contains_GreaterThan(bool useParameterizedQueries, string contains, string greaterThan,
                int mode, string trueValue, string falseValue)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _filter.Name.Contains = contains;
                _filter.Name.GreaterThan = greaterThan;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Or(entity => entity.Name, filter => filter.Name.Contains, filter => filter.Name.GreaterThan);

                AssertNameSpecification(specification, trueValue, falseValue);
            }

            [Theory]
            [InlineData(false, "AbC", "fgh", 0, "pqrAbCxyz", "cde")]      // StringComparisonMode.None
            [InlineData(true, "AbC", "fgh", 0, "pqrAbCxyz", "cde")]
            [InlineData(false, "AbC", "fgh", 1, "pqrAbCxyz", "cde")]      // StringComparisonMode.ToLower
            [InlineData(true, "AbC", "fgh", 1, "pqrAbCxyz", "cde")]
            [InlineData(false, "AbC", "fgh", 2, "pqrAbCxyz", "cde")]      // StringComparisonMode.ToUpper
            [InlineData(true, "AbC", "fgh", 2, "pqrAbCxyz", "cde")]
            [InlineData(false, "AbC", "fgh", 6, "pqrAbCxyz", "cde")]      // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, "AbC", "fgh", 6, "pqrAbCxyz", "cde")]
            public void Should_Or_GreaterThan_Contains(bool useParameterizedQueries, string contains, string greaterThan,
                int mode, string trueValue, string falseValue)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _filter.Name.Contains = contains;
                _filter.Name.GreaterThan = greaterThan;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Or(entity => entity.Name, filter => filter.Name.GreaterThan, filter => filter.Name.Contains);

                AssertNameSpecification(specification, trueValue, falseValue);
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Or_EqualTo_In(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Or(entity => entity.Name, filter => filter.Name.EqualTo, filter => filter.Name.In);

                var trueValue = _filter.Name.EqualTo.Value;
                var falseValue = Create<string>();

                AssertNameSpecification(specification, trueValue, falseValue);
            }

            [Theory]
            [InlineData(false, 0)]  // StringComparisonMode.None
            [InlineData(true, 0)]
            [InlineData(false, 1)]  // StringComparisonMode.ToLower
            [InlineData(true, 1)]
            [InlineData(false, 2)]  // StringComparisonMode.ToUpper
            [InlineData(true, 2)]
            [InlineData(false, 6)]  // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, 6)]
            public void Should_Or_NotIn_NotEqualTo(bool useParameterizedQueries, int mode)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _filter.Name.NotIn.Value.Add(_filter.Name.NotEqualTo.Value);

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Or(entity => entity.Name, filter => filter.Name.NotIn, filter => filter.Name.NotEqualTo);

                var trueValue = Create<string>();
                var falseValue = _filter.Name.NotEqualTo.Value;

                AssertNameSpecification(specification, trueValue, falseValue);
            }

            [Fact]
            public void Should_Ignore_Null_Filter()
            {
                _options = new DefaultQueryFilterOptions
                {
                    IgnoreNullFilterValues = true
                };

                _filter.Name.EqualTo.Value = null;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Or(entity => entity.Name, filter => filter.Name.EqualTo, filter => filter.Name.StartsWith);

                var entity = new DummyEntity
                {
                    Name = $"{_filter.Name.StartsWith.Value}ZZZ"
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();
            }
        }

        public class Or_StringFilter : FilterSpecificationBuilderFixture
        {
            [Theory]
            [InlineData(false, "abc", "123", 0, "xyz_abc_fgh", "pqr")]      // StringComparisonMode.None
            [InlineData(true, "123", "xyz", 0, "xyz_abc_fgh", "pqr")]
            [InlineData(false, "abc", "123", 1, "xyz_abc_fgh", "pqr")]      // StringComparisonMode.ToLower
            [InlineData(true, "123", "xyz", 1, "xyz_abc_fgh", "pqr")]
            [InlineData(false, "abc", "123", 2, "xyz_abc_fgh", "pqr")]      // StringComparisonMode.ToUpper
            [InlineData(true, "123", "xyz", 2, "xyz_abc_fgh", "pqr")]
            [InlineData(false, "abc", "123", 6, "xyz_abc_fgh", "pqr")]      // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, "123", "xyz", 6, "xyz_abc_fgh", "pqr")]
            public void Should_Or_Contains_StartsWith(bool useParameterizedQueries, string contains, string startsWith,
               int mode, string trueValue, string falseValue)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _filter.Name.Contains = contains;
                _filter.Name.StartsWith = startsWith;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Or(entity => entity.Name, filter => filter.Name.Contains, filter => filter.Name.StartsWith);

                AssertNameSpecification(specification, trueValue, falseValue);
            }

            [Theory]
            [InlineData(false, "123", "abc", 0, "pqrxyz", "123")]   // StringComparisonMode.None
            [InlineData(true, "abc", "123", 0, "pqrxyz", "abc")]
            [InlineData(false, "123", "xyz", 1, "pqrxyz", "123")]   // StringComparisonMode.ToLower
            [InlineData(true, "abc", "123", 1, "pqrxyz", "abc")]
            [InlineData(false, "123", "xyz", 2, "pqrxyz", "123")]   // StringComparisonMode.ToUpper
            [InlineData(true, "abc", "123", 2, "pqrxyz", "abc")]
            [InlineData(false, "123", "xyz", 6, "pqrxyz", "123")]   // StringComparisonMode.InvariantCultureIgnoreCase
            [InlineData(true, "abc", "123", 6, "pqrxyz", "abc")]
            public void Should_Or_NotContains_EndsWith(bool useParameterizedQueries, string notContains, string endsWith,
               int mode, string trueValue, string falseValue)
            {
                var stringComparisonMode = StringComparisonMode.From(mode);

                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparisonMode = stringComparisonMode,
                    IgnoreNullFilterValues = false
                };

                _filter.Name.NotContains = notContains;
                _filter.Name.EndsWith = endsWith;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Or(entity => entity.Name, filter => filter.Name.NotContains, filter => filter.Name.EndsWith);

                AssertNameSpecification(specification, trueValue, falseValue);
            }

            [Fact]
            public void Should_Ignore_Null_Filter()
            {
                _options = new DefaultQueryFilterOptions
                {
                    IgnoreNullFilterValues = true
                };

                _filter.Name.EqualTo.Value = null;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Or(entity => entity.Name, filter => filter.Name.EqualTo, filter => filter.Name.StartsWith);

                var entity = new DummyEntity
                {
                    Name = $"{_filter.Name.StartsWith.Value}ZZZ"
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();
            }
        }

        public class Or_Value : FilterSpecificationBuilderFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Or_LessThanOrEqual_GreaterThanOrEqual(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var lastUpdated = DateTime.Now.Date;

                _filter.LastUpdated.LessThanOrEqual = DateTime.MinValue;
                _filter.LastUpdated.GreaterThanOrEqual = lastUpdated.AddDays(-1);

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Or(
                    entity => entity.LastUpdated,
                    filter => filter.LastUpdated.LessThanOrEqual,
                    filter => filter.LastUpdated.GreaterThanOrEqual);

                var entityPrice = _filter.Price.In.Value[2];

                AssertLastUpdatedSpecification(specification, lastUpdated, lastUpdated.AddDays(-2));
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Or_EqualTo_GreaterThanOrEqual(bool useParameterizedQueries)
            {
                _options = new DefaultQueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    IgnoreNullFilterValues = false
                };

                var entityPrice = Create<double>();

                _filter.Price.EqualTo = entityPrice;
                _filter.Price.GreaterThanOrEqual = 0;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Or(
                    entity => entity.Price,
                    filter => filter.Price.EqualTo,
                    filter => filter.Price.GreaterThanOrEqual);

                AssertPriceSpecification(specification, entityPrice, -entityPrice);
            }

            [Fact]
            public void Should_Ignore_Null_Filter()
            {
                _options = new DefaultQueryFilterOptions
                {
                    IgnoreNullFilterValues = true
                };

                _filter.LastUpdated.GreaterThanOrEqual.Value = null;

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Or(entity => entity.LastUpdated, filter => filter.LastUpdated.LessThanOrEqual, filter => filter.LastUpdated.GreaterThanOrEqual);

                var entity = new DummyEntity
                {
                    LastUpdated = _filter.LastUpdated.LessThanOrEqual.Value.AddDays(-1)
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();
            }
        }

        private void AssertNameSpecification(ILinqSpecification<DummyEntity> specification, string trueValue, string falseValue)
        {
            if (_options.StringComparisonMode == StringComparisonMode.ToLower)
            {
                trueValue = trueValue?.ToLower();
            }

            var entity = new DummyEntity
            {
                Name = trueValue
            };

            specification.IsSatisfiedBy(entity).Should().BeTrue();

            entity.Name = falseValue;

            specification.IsSatisfiedBy(entity).Should().BeFalse();
        }

        private void AssertPriceSpecification(ILinqSpecification<DummyEntity> specification, double? trueValue, double? falseValue)
        {
            var entity = new DummyEntity
            {
                Price = trueValue
            };

            specification.IsSatisfiedBy(entity).Should().BeTrue();

            entity.Price = falseValue;

            specification.IsSatisfiedBy(entity).Should().BeFalse();
        }

        private void AssertLastUpdatedSpecification(ILinqSpecification<DummyEntity> specification, DateTime? trueValue, DateTime? falseValue)
        {
            var entity = new DummyEntity
            {
                LastUpdated = trueValue
            };

            specification.IsSatisfiedBy(entity).Should().BeTrue();

            entity.LastUpdated = falseValue;

            specification.IsSatisfiedBy(entity).Should().BeFalse();
        }
    }
}
