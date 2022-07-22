using AllOverIt.Extensions;
using AllOverIt.Filtering.Builders;
using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Options;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;
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
            public DateTime DateCreated { get; set; }
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
            }

            public sealed class NameFilter
            {
                public NotContains NotContains { get; set; } = new();
                public Contains Contains { get; set; } = new();
                public StartsWith StartsWith { get; set; } = new();
                public EndsWith EndsWith { get; set; } = new();
                public In<string> In { get; set; } = new();
                public NotIn<string> NotIn { get; set; } = new();
                public GreaterThan<string> GreaterThan { get; set; } = new();
                public GreaterThanOrEqual<string> GreaterThanOrEqual { get; set; } = new();
                public LessThan<string> LessThan { get; set; } = new();
                public LessThanOrEqual<string> LessThanOrEqual { get; set; } = new();
            }

            public sealed class PriceFilter
            {
                public LessThanOrEqual<double> LessThanOrEqual { get; set; } = new();
                public GreaterThanOrEqual<double?> GreaterThanOrEqual { get; set; } = new();
            }

            public sealed class DateCreatedFilter
            {
                public LessThanOrEqual<DateTime> LessThanOrEqual { get; set; } = new();
                public GreaterThanOrEqual<DateTime?> GreaterThanOrEqual { get; set; } = new();
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
            public DateCreatedFilter DateCreated { get; init; } = new();
            public LastUpdatedFilter LastUpdated { get; init; } = new();
        }

        public class Constructor : FilterSpecificationBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Filter_Null()
            {
                Invoking(() =>
                {
                    _ = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(null, A.Fake<IQueryFilterOptions>());
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
            private DummyEntityFilter _filter;
            private IQueryFilterOptions _options;
            private IFilterSpecificationBuilder<DummyEntity, DummyEntityFilter> _specificationBuilder;

            public Create_StringFilter()
            {
                _filter = new DummyEntityFilter
                {
                    Active =
                    {
                        EqualTo = Create<bool>()
                    },
                    Category =
                    { 
                        EqualTo = Create<DummyEntityCategory>()
                    },
                    Name =
                    {
                        NotContains = Create<string>(),
                        Contains = Create<string>(),
                        StartsWith = Create<string>(),
                        EndsWith = Create<string>(),
                        In = CreateMany<string>().ToList(),
                        NotIn = CreateMany<string>().ToList(),
                        GreaterThan = Create<string>(),
                        GreaterThanOrEqual = Create<string>(),
                        LessThan = Create<string>(),
                        LessThanOrEqual = Create<string>()
                    },
                    Price =
                    {
                        LessThanOrEqual = Create<double>(),
                        GreaterThanOrEqual = Create<double>()
                    },
                    DateCreated =
                    {
                        LessThanOrEqual = DateTime.UtcNow,
                        GreaterThanOrEqual = DateTime.UtcNow.AddDays(-7)
                    }
                };

                // all default options
                _options = new QueryFilterOptions
                {
                    UseParameterizedQueries = Create<bool>(),
                    StringComparison = Create<bool>() ? default : StringComparison.InvariantCultureIgnoreCase,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);
            }

            [Theory]
            [InlineData(false, default)]
            [InlineData(true, default)]
            [InlineData(false, StringComparison.InvariantCultureIgnoreCase)]
            [InlineData(true, StringComparison.InvariantCultureIgnoreCase)]
            public void Should_Create_Contains(bool useParameterizedQueries, StringComparison? stringComparison)
            {
                _options = new QueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparison = stringComparison,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.Contains);

                var entityName = $"{Create<string>()}{_filter.Name.Contains.Value}{Create<string>()}";

                if (_options.StringComparison == StringComparison.InvariantCultureIgnoreCase)
                {
                    entityName = entityName.ToLower();
                }

                var entity = new DummyEntity
                {
                    Name = entityName
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();

                entity.Name = Create<string>();

                specification.IsSatisfiedBy(entity).Should().BeFalse();
            }

            [Theory]
            [InlineData(false, default)]
            [InlineData(true, default)]
            [InlineData(false, StringComparison.InvariantCultureIgnoreCase)]
            [InlineData(true, StringComparison.InvariantCultureIgnoreCase)]
            public void Should_Create_NotContains(bool useParameterizedQueries, StringComparison? stringComparison)
            {
                _options = new QueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparison = stringComparison,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.NotContains);

                var entityName = $"{Create<string>()}{_filter.Name.NotContains.Value}{Create<string>()}";

                if (_options.StringComparison == StringComparison.InvariantCultureIgnoreCase)
                {
                    entityName = entityName.ToLower();
                }

                var entity = new DummyEntity
                {
                    Name = entityName
                };

                specification.IsSatisfiedBy(entity).Should().BeFalse();

                entity.Name = Create<string>();

                specification.IsSatisfiedBy(entity).Should().BeTrue();
            }

            [Theory]
            [InlineData(false, default)]
            [InlineData(true, default)]
            [InlineData(false, StringComparison.InvariantCultureIgnoreCase)]
            [InlineData(true, StringComparison.InvariantCultureIgnoreCase)]
            public void Should_Create_StartsWith(bool useParameterizedQueries, StringComparison? stringComparison)
            {
                _options = new QueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparison = stringComparison,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.StartsWith);

                var entityName = $"{_filter.Name.StartsWith.Value}{Create<string>()}";

                if (_options.StringComparison == StringComparison.InvariantCultureIgnoreCase)
                {
                    entityName = entityName.ToLower();
                }

                var entity = new DummyEntity
                {
                    Name = entityName
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();

                entity.Name = $"{Create<string>()}{_filter.Name.StartsWith.Value}";

                specification.IsSatisfiedBy(entity).Should().BeFalse();
            }

            [Theory]
            [InlineData(false, default)]
            [InlineData(true, default)]
            [InlineData(false, StringComparison.InvariantCultureIgnoreCase)]
            [InlineData(true, StringComparison.InvariantCultureIgnoreCase)]
            public void Should_Create_EndsWith(bool useParameterizedQueries, StringComparison? stringComparison)
            {
                _options = new QueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparison = stringComparison,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.EndsWith);

                var entityName = $"{Create<string>()}{_filter.Name.EndsWith.Value}";

                if (_options.StringComparison == StringComparison.InvariantCultureIgnoreCase)
                {
                    entityName = entityName.ToLower();
                }

                var entity = new DummyEntity
                {
                    Name = entityName
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();

                entity.Name = $"{_filter.Name.EndsWith.Value}{Create<string>()}";

                specification.IsSatisfiedBy(entity).Should().BeFalse();
            }

            [Theory]
            [InlineData(false, default)]
            [InlineData(true, default)]
            [InlineData(false, StringComparison.InvariantCultureIgnoreCase)]
            [InlineData(true, StringComparison.InvariantCultureIgnoreCase)]
            public void Should_Create_In(bool useParameterizedQueries, StringComparison? stringComparison)
            {
                _options = new QueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparison = stringComparison,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.In);

                var entityName = _filter.Name.In.Value[2];

                if (_options.StringComparison == StringComparison.InvariantCultureIgnoreCase)
                {
                    entityName = entityName.ToLower();
                }

                var entity = new DummyEntity
                {
                    Name = entityName
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();

                entity.Name = Create<string>();

                specification.IsSatisfiedBy(entity).Should().BeFalse();
            }

            [Theory]
            [InlineData(false, default)]
            [InlineData(true, default)]
            [InlineData(false, StringComparison.InvariantCultureIgnoreCase)]
            [InlineData(true, StringComparison.InvariantCultureIgnoreCase)]
            public void Should_Create_NotIn(bool useParameterizedQueries, StringComparison? stringComparison)
            {
                _options = new QueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparison = stringComparison,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.NotIn);

                var entityName = _filter.Name.NotIn.Value[2];

                if (_options.StringComparison == StringComparison.InvariantCultureIgnoreCase)
                {
                    entityName = entityName.ToLower();
                }

                var entity = new DummyEntity
                {
                    Name = entityName
                };

                specification.IsSatisfiedBy(entity).Should().BeFalse();

                entity.Name = Create<string>();

                specification.IsSatisfiedBy(entity).Should().BeTrue();
            }

            [Theory]
            [InlineData(false, default)]
            [InlineData(true, default)]
            [InlineData(false, StringComparison.InvariantCultureIgnoreCase)]
            [InlineData(true, StringComparison.InvariantCultureIgnoreCase)]
            public void Should_Create_GreaterThan(bool useParameterizedQueries, StringComparison? stringComparison)
            {
                _options = new QueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparison = stringComparison,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.GreaterThan);

                var entityName = $"{_filter.Name.GreaterThan.Value}ZZZ";

                if (_options.StringComparison == StringComparison.InvariantCultureIgnoreCase)
                {
                    entityName = entityName.ToLower();
                }

                var entity = new DummyEntity
                {
                    Name = entityName
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();

                entity.Name = null;

                specification.IsSatisfiedBy(entity).Should().BeFalse();
            }

            [Theory]
            [InlineData(false, default)]
            [InlineData(true, default)]
            [InlineData(false, StringComparison.InvariantCultureIgnoreCase)]
            [InlineData(true, StringComparison.InvariantCultureIgnoreCase)]
            public void Should_Create_GreaterThanOrEqual(bool useParameterizedQueries, StringComparison? stringComparison)
            {
                _options = new QueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparison = stringComparison,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.GreaterThanOrEqual);

                var entityName = $"{_filter.Name.GreaterThanOrEqual.Value}ZZZ";

                if (_options.StringComparison == StringComparison.InvariantCultureIgnoreCase)
                {
                    entityName = entityName.ToLower();
                }

                var entity = new DummyEntity
                {
                    Name = entityName
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();

                entity.Name = null;

                specification.IsSatisfiedBy(entity).Should().BeFalse();
            }

            [Theory]
            [InlineData(false, default)]
            [InlineData(true, default)]
            [InlineData(false, StringComparison.InvariantCultureIgnoreCase)]
            [InlineData(true, StringComparison.InvariantCultureIgnoreCase)]
            public void Should_Create_GreaterThanOrEqual_When_Equal(bool useParameterizedQueries, StringComparison? stringComparison)
            {
                _options = new QueryFilterOptions
                {
                    UseParameterizedQueries = useParameterizedQueries,
                    StringComparison = stringComparison,
                    IgnoreNullFilterValues = false
                };

                _specificationBuilder = new FilterSpecificationBuilder<DummyEntity, DummyEntityFilter>(_filter, _options);

                var specification = _specificationBuilder.Create(entity => entity.Name, filter => filter.Name.GreaterThanOrEqual);

                var entityName = _filter.Name.GreaterThanOrEqual.Value;

                if (_options.StringComparison == StringComparison.InvariantCultureIgnoreCase)
                {
                    entityName = entityName.ToLower();
                }

                var entity = new DummyEntity
                {
                    Name = entityName
                };

                specification.IsSatisfiedBy(entity).Should().BeTrue();

                entity.Name = null;

                specification.IsSatisfiedBy(entity).Should().BeFalse();
            }


        }
    }
}
