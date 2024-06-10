using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using FluentAssertions;
using static AllOverIt.Mapping.Tests.ObjectMapperTypes;

namespace AllOverIt.Mapping.Tests
{
    public class ObjectMapperConfigurationFixture : FixtureBase
    {
        private readonly ObjectMapperConfiguration _objectMapperConfiguration = new();

        public class Constructor_Default : ObjectMapperConfigurationFixture
        {
            private readonly PropertyMatcherCache _propertyMatcherCache = new();

            [Fact]
            public void Should_Set_ObjectMapperOptions()
            {
                _objectMapperConfiguration.Options.Should().NotBeNull();
            }

            [Fact]
            public void Should_Set_PropertyMatcherCache()
            {
                _objectMapperConfiguration._propertyMatcherCache.Should().NotBeNull();
            }

            [Fact]
            public void Should_Set_ObjectMapperTypeFactory()
            {
                _objectMapperConfiguration._typeFactory.Should().NotBeNull();
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
                var expected = new ObjectMapperOptions(new ObjectMapperTypeFactory());

                var configuration = new ObjectMapperConfiguration(options =>
                {
                    options.AllowNullCollections = expected.AllowNullCollections;
                });

                expected.Should().BeEquivalentTo(configuration.Options);
            }

            [Fact]
            public void Should_Set_PropertyMatcherCache()
            {
                _objectMapperConfiguration._propertyMatcherCache.Should().NotBeNull();
            }

            [Fact]
            public void Should_Set_ObjectMapperTypeFactory()
            {
                _objectMapperConfiguration._typeFactory.Should().NotBeNull();
            }
        }

        public class Configure : ObjectMapperConfigurationFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Configure_Null()
            {
                Invoking(() =>
                {
                    _objectMapperConfiguration.Configure<DummySource2, DummyTarget>(null);
                })
                   .Should()
                   .NotThrow();
            }

            [Fact]
            public void Should_Default_Configure()
            {
                _objectMapperConfiguration.Configure<DummySource2, DummyTarget>();

                _objectMapperConfiguration._propertyMatcherCache
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

                _objectMapperConfiguration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Binding = binding;

                    options.WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value);
                });

                _objectMapperConfiguration._propertyMatcherCache
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
                _objectMapperConfiguration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                    options.WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value);
                });

                _objectMapperConfiguration._propertyMatcherCache
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
                _objectMapperConfiguration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .Exclude(src => src.Prop10);
                });

                _objectMapperConfiguration._propertyMatcherCache
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
                _objectMapperConfiguration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .WithAlias(src => src.Prop8, trg => trg.Prop1)
                        .WithAlias(src => (int) src.Prop12, trg => trg.Prop5);
                });

                _objectMapperConfiguration._propertyMatcherCache
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
                var factor = GetWithinRange(2, 5);
                IObjectMapper actualMapper = null;

                _objectMapperConfiguration.Configure<DummySource2, DummyTarget>(options =>
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

                _objectMapperConfiguration._propertyMatcherCache
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

        public class GetOrAdd : ObjectMapperConfigurationFixture
        {
            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                {
                    _objectMapperConfiguration.GetOrAdd(null, () => new { });
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Throw_When_Factory_Null()
            {
                Invoking(() =>
                {
                    _objectMapperConfiguration.GetOrAdd(typeof(DummySource1), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("factory");
            }

            [Fact]
            public void Should_Add_Factory()
            {
                var added = false;

                var actual = _objectMapperConfiguration
                    .GetOrAdd(typeof(DummySource1), () =>
                    {
                        added = true;

                        return new DummySource1();
                    })
                    .Invoke();

                added.Should().BeTrue();

                actual.Should().BeOfType<DummySource1>();
            }

            [Fact]
            public void Should_Get_From_Factory()
            {
                var added = 0;

                var actual = _objectMapperConfiguration
                    .GetOrAdd(typeof(DummySource1), () =>
                    {
                        added++;

                        return new DummySource1();
                    })
                    .Invoke();

                added.Should().Be(1);

                actual.Should().BeOfType<DummySource1>();

                actual = _objectMapperConfiguration
                    .GetOrAdd(typeof(DummySource1), () =>
                    {
                        return null;
                    })
                    .Invoke();

                actual.Should().BeOfType<DummySource1>();
            }
        }

        public class GetTypeFactory_Generic : ObjectMapperConfigurationFixture
        {
            [Fact]
            public void Should_Get_Same_Factory()
            {
                var factory1 = _objectMapperConfiguration.GetTypeFactory<DummySource1>();
                var factory2 = _objectMapperConfiguration.GetTypeFactory<DummySource1>();

                factory1.Should().BeSameAs(factory2);
            }

            [Fact]
            public void Should_Get_From_Factory()
            {
                var actual = _objectMapperConfiguration
                   .GetTypeFactory<DummySource1>()
                   .Invoke();

                actual.Should().BeOfType<DummySource1>();
            }
        }

        public class GetTypeFactory_Type : ObjectMapperConfigurationFixture
        {
            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                {
                    _objectMapperConfiguration.GetTypeFactory(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Get_Same_Factory()
            {
                var factory1 = _objectMapperConfiguration.GetTypeFactory(typeof(DummySource1));
                var factory2 = _objectMapperConfiguration.GetTypeFactory(typeof(DummySource1));

                factory1.Should().BeSameAs(factory2);
            }

            [Fact]
            public void Should_Get_From_Factory()
            {
                var actual = _objectMapperConfiguration
                   .GetTypeFactory(typeof(DummySource1))
                   .Invoke();

                actual.Should().BeOfType<DummySource1>();
            }
        }
    }
}