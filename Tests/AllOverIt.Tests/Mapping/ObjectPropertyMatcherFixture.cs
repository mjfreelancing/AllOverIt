using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Mapping;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

using static AllOverIt.Tests.Mapping.ObjectMapperTypes;

namespace AllOverIt.Tests.Mapping
{
    public class ObjectPropertyMatcherFixture : FixtureBase
    {
        public class Constructor : ObjectPropertyMatcherFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Type_Null()
            {
                Invoking(() =>
                {
                    _ = new ObjectPropertyMatcher(null, typeof(DummyTarget), Create<PropertyMatcherOptions>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceType");
            }

            [Fact]
            public void Should_Throw_When_Target_Type_Null()
            {
                Invoking(() =>
                {
                    _ = new ObjectPropertyMatcher(typeof(DummySource2), null, Create<PropertyMatcherOptions>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("targetType");
            }

            [Fact]
            public void Should_Throw_When_PropertyMatcherOptions_Null()
            {
                Invoking(() =>
                {
                    _ = new ObjectPropertyMatcher(typeof(DummySource2), typeof(DummyTarget), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("matcherOptions");
            }

            [Fact]
            public void Should_Use_Provided_Matcher_Options()
            {
                var options = Create<PropertyMatcherOptions>();
                var actual = new ObjectPropertyMatcher(typeof(DummySource1), typeof(DummyTarget), options);

                options.Should().BeSameAs(actual.MatcherOptions);
            }

            [Fact]
            public void Should_Get_Default_Mapping()
            {
                var defaultOptions = new PropertyMatcherOptions();
                var actual = new ObjectPropertyMatcher(typeof(DummySource2), typeof(DummyTarget), defaultOptions);

                var actualMatches = GetMatchesNameAndType(actual.Matches);

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
                var options = new PropertyMatcherOptions();

                options.Binding = BindingOptions.Instance | BindingOptions.Internal;

                options.WithConversion(nameof(DummySource2.Prop13), (mapper, value) => (DummyEnum) value);

                var actual = new ObjectPropertyMatcher(typeof(DummySource2), typeof(DummyTarget), options);

                var actualMatches = GetMatchesNameAndType(actual.Matches);

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
                var options = new PropertyMatcherOptions();

                options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                options.WithConversion(nameof(DummySource2.Prop13), (mapper, value) => (DummyEnum) value);

                var actual = new ObjectPropertyMatcher(typeof(DummySource2), typeof(DummyTarget), options);

                var actualMatches = GetMatchesNameAndType(actual.Matches);

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
                var options = new PropertyMatcherOptions();

                options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                options.WithConversion(nameof(DummySource2.Prop13), (mapper, value) => (DummyEnum) value)
                       .Exclude(nameof(DummySource2.Prop10));

                var actual = new ObjectPropertyMatcher(typeof(DummySource2), typeof(DummyTarget), options);

                var actualMatches = GetMatchesNameAndType(actual.Matches);

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
                var options = new PropertyMatcherOptions();

                options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                options.WithConversion(nameof(DummySource2.Prop13), (mapper, value) => (DummyEnum) value)
                        .WithAlias(nameof(DummySource2.Prop8), nameof(DummyTarget.Prop1))
                        .WithAlias(nameof(DummySource2.Prop12), nameof(DummyTarget.Prop5));

                var actual = new ObjectPropertyMatcher(typeof(DummySource2), typeof(DummyTarget), options);

                var actualMatches = GetMatchesNameAndType(actual.Matches);

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

                var options = new PropertyMatcherOptions();

                options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                options.WithConversion(nameof(DummySource2.Prop13), (mapper, value) => (DummyEnum) value)
                        .WithAlias(nameof(DummySource2.Prop8), nameof(DummyTarget.Prop1))
                        .WithAlias(nameof(DummySource2.Prop12), nameof(DummyTarget.Prop5));

                options.WithConversion(nameof(DummySource2.Prop8), (mapper, value) =>
                {
                    actualMapper = mapper;
                    return (int)value * factor;
                });

                var actual = new ObjectPropertyMatcher(typeof(DummySource2), typeof(DummyTarget), options);

                var actualMatches = GetMatchesNameAndType(actual.Matches);

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
                var convertedValue = options.GetConvertedValue(mapper, nameof(DummySource2.Prop8), value);

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