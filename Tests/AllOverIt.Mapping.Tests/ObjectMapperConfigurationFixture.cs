using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

using static AllOverIt.Mapping.Tests.ObjectMapperTypes;

namespace AllOverIt.Mapping.Tests
{
    public class ObjectMapperConfigurationFixture : FixtureBase
    {
        public class Constructor_Default : ObjectMapperConfigurationFixture
        {
            private readonly PropertyMatcherCache _propertyMatcherCache = new();

            [Fact]
            public void Should_Set_ObjectMapperOptions()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Options.Should().NotBeNull();
            }

            [Fact]
            public void Should_Set_PropertyMatcherCache()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration._propertyMatcherCache.Should().NotBeNull();
            }

            [Fact]
            public void Should_Set_ObjectMapperTypeFactory()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration._typeFactory.Should().NotBeNull();
            }
        }

        public class Constructor_Options_Action : ObjectMapperConfigurationFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = new ObjectMapperConfiguration(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("configure");
            }

            [Fact]
            public void Should_Set_ObjectMapperOptions()
            {
                var expected = Create<ObjectMapperOptions>();

                var configuration = new ObjectMapperConfiguration(options =>
                {
                    options.AllowNullCollections = expected.AllowNullCollections;
                });

                expected.Should().BeEquivalentTo(configuration.Options);
            }

            [Fact]
            public void Should_Set_PropertyMatcherCache()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration._propertyMatcherCache.Should().NotBeNull();
            }

            [Fact]
            public void Should_Set_ObjectMapperTypeFactory()
            {
                var configuration = new ObjectMapperConfiguration(options => { });

                configuration._typeFactory.Should().NotBeNull();
            }
        }

        public class Configure : ObjectMapperConfigurationFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Configure_Null()
            {
                Invoking(() =>
                {
                    var configuration = new ObjectMapperConfiguration();
                    configuration.Configure<DummySource2, DummyTarget>(null);
                })
                   .Should()
                   .NotThrow();
            }

            [Fact]
            public void Should_Default_Configure()
            {
                var configuration = new ObjectMapperConfiguration();
                configuration.Configure<DummySource2, DummyTarget>();

                configuration._propertyMatcherCache
                    .TryGetMapper(typeof(DummySource2), typeof(DummyTarget), out var propertyMatcher)
                    .Should()
                    .BeTrue();

                propertyMatcher.MatcherOptions.Should().BeSameAs(PropertyMatcherOptions.None);

                var actualMatches = GetMatchesNameAndType(propertyMatcher.Matches);

                var expected = new[]
                {
                    (nameof(DummySource2.Prop1), typeof(int), nameof(DummyTarget.Prop1), typeof(float)),
                    (nameof(DummySource2.Prop3), typeof(string), nameof(DummyTarget.Prop3), typeof(string)),
                    (nameof(DummySource2.Prop5), typeof(int?), nameof(DummyTarget.Prop5), typeof(int)),
                    (nameof(DummySource2.Prop6), typeof(int), nameof(DummyTarget.Prop6), typeof(int?)),
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop8), typeof(int)),
                    (nameof(DummySource2.Prop9), typeof(IEnumerable<string>), nameof(DummyTarget.Prop9), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop10), typeof(IReadOnlyCollection<string>), nameof(DummyTarget.Prop10), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop11), typeof(IEnumerable<string>), nameof(DummyTarget.Prop11), typeof(IReadOnlyCollection<string>)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop12), typeof(int)),
                    (nameof(DummySource2.Prop13), typeof(int), nameof(DummyTarget.Prop13), typeof(DummyEnum))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }

            [Fact]
            public void Should_Configure_With_Custom_Bindings()
            {
                var binding = BindingOptions.Instance | BindingOptions.Internal;

                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Binding = binding;

                    options.WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value);
                });

                configuration._propertyMatcherCache
                   .TryGetMapper(typeof(DummySource2), typeof(DummyTarget), out var propertyMatcher)
                   .Should()
                   .BeTrue();

                propertyMatcher.MatcherOptions.Binding.Should().Be(binding);

                var actualMatches = GetMatchesNameAndType(propertyMatcher.Matches);

                var expected = new[]
                {
                    (nameof(DummySource2.Prop4), typeof(int), nameof(DummyTarget.Prop4), typeof(int))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }

            [Fact]
            public void Should_Configure_With_Filter()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                    options.WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value);
                });

                configuration._propertyMatcherCache
                  .TryGetMapper(typeof(DummySource2), typeof(DummyTarget), out var propertyMatcher)
                  .Should()
                  .BeTrue();

                var actualMatches = GetMatchesNameAndType(propertyMatcher.Matches);

                var expected = new[]
                {
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop8), typeof(int)),
                    (nameof(DummySource2.Prop10), typeof(IReadOnlyCollection<string>), nameof(DummyTarget.Prop10), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop12), typeof(int))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }

            [Fact]
            public void Should_Configure_With_Exclude()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .Exclude(src => src.Prop10);
                });

                configuration._propertyMatcherCache
                  .TryGetMapper(typeof(DummySource2), typeof(DummyTarget), out var propertyMatcher)
                  .Should()
                  .BeTrue();

                var actualMatches = GetMatchesNameAndType(propertyMatcher.Matches);

                var expected = new[]
                {
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop8), typeof(int)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop12), typeof(int))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }

            [Fact]
            public void Should_Configure_With_Filter_And_Alias()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .WithAlias(src => src.Prop8, trg => trg.Prop1)
                        .WithAlias(src => (int) src.Prop12, trg => trg.Prop5);
                });

                configuration._propertyMatcherCache
                  .TryGetMapper(typeof(DummySource2), typeof(DummyTarget), out var propertyMatcher)
                  .Should()
                  .BeTrue();

                var actualMatches = GetMatchesNameAndType(propertyMatcher.Matches);

                var expected = new[]
                {
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop1), typeof(float)),
                    (nameof(DummySource2.Prop10), typeof(IReadOnlyCollection<string>), nameof(DummyTarget.Prop10), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop5), typeof(int))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }

            [Fact]
            public void Should_Configure_With_Filter_And_Alias_And_Conversion()
            {
                var configuration = new ObjectMapperConfiguration();

                var factor = GetWithinRange(2, 5);
                IObjectMapper actualMapper = null;

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .WithAlias(src => src.Prop8, trg => trg.Prop1)
                        .WithAlias(src => (int) src.Prop12, trg => trg.Prop5);

                    options.WithConversion(src => src.Prop8, (mapper, value) =>
                    {
                        actualMapper = mapper;
                        return value * factor;
                    });
                });

                configuration._propertyMatcherCache
                  .TryGetMapper(typeof(DummySource2), typeof(DummyTarget), out var propertyMatcher)
                  .Should()
                  .BeTrue();

                var actualMatches = GetMatchesNameAndType(propertyMatcher.Matches);

                var expectedAliases = new[]
                {
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop1), typeof(float)),
                    (nameof(DummySource2.Prop10), typeof(IReadOnlyCollection<string>), nameof(DummyTarget.Prop10), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop5), typeof(int))
                };

                expectedAliases
                    .Should()
                    .BeEquivalentTo(actualMatches);

                var value = Create<int>() % 1000 + 1;

                var mapper = new ObjectMapper();
                var convertedValue = propertyMatcher.MatcherOptions.GetConvertedValue(mapper, nameof(DummySource2.Prop8), value);

                actualMapper.Should().BeSameAs(mapper);         // Just an additional sanity check
                convertedValue.Should().Be(value * factor);
            }

            private static IEnumerable<(string SourceName, Type SourceType, string TargetName, Type TargetType)>
                GetMatchesNameAndType(IEnumerable<ObjectPropertyMatcher.PropertyMatchInfo> matches)
            {
                return matches.Select(
                    match => (match.SourceInfo.Name, match.SourceInfo.PropertyType,
                              match.TargetInfo.Name, match.TargetInfo.PropertyType));
            }
        }
    }
}