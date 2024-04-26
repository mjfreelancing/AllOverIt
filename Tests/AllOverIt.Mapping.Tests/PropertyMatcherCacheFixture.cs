using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Mapping.Exceptions;
using FluentAssertions;

using static AllOverIt.Mapping.Tests.ObjectMapperTypes;

namespace AllOverIt.Mapping.Tests
{
    public class PropertyMatcherCacheFixture : FixtureBase
    {
        public class CreateMapper : PropertyMatcherCacheFixture
        {
            private readonly PropertyMatcherCache _cache = new();

            [Fact]
            public void Should_Throw_When_Source_Type_Null()
            {
                Invoking(() => _cache.CreateMapper(null, typeof(DummyTarget), Create<PropertyMatcherOptions>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceType");
            }

            [Fact]
            public void Should_Throw_When_Target_Type_Null()
            {
                Invoking(() => _cache.CreateMapper(typeof(DummySource2), null, Create<PropertyMatcherOptions>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("targetType");
            }

            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() => _cache.CreateMapper(typeof(DummySource2), typeof(DummyTarget), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("matcherOptions");
            }


            [Fact]
            public void Should_Throw_When_Configured_More_Than_Once()
            {
                _cache.CreateMapper(typeof(DummySource2), typeof(DummyTarget), Create<PropertyMatcherOptions>());

                Invoking(() => _cache.CreateMapper(typeof(DummySource2), typeof(DummyTarget), Create<PropertyMatcherOptions>()))
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage($"Mapping already exists between {nameof(DummySource2)} and {nameof(DummyTarget)}.");
            }

            [Fact]
            public void Should_Create_Mapper_With_Options()
            {
                _cache.TryGetMapper(typeof(DummySource2), typeof(DummyTarget), out _).Should().BeFalse();

                var matcherOptions = Create<PropertyMatcherOptions>();

                var actual = _cache.CreateMapper(typeof(DummySource2), typeof(DummyTarget), matcherOptions);

                actual.Should().NotBeNull();
                actual.MatcherOptions.Should().BeSameAs(matcherOptions);
            }
        }

        public class GetOrCreateMapper : PropertyMatcherCacheFixture
        {
            private readonly PropertyMatcherCache _cache = new();

            [Fact]
            public void Should_Throw_When_Source_Type_Null()
            {
                Invoking(() => _ = _cache.GetOrCreateMapper(null, typeof(DummyTarget)))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceType");
            }

            [Fact]
            public void Should_Throw_When_Target_Type_Null()
            {
                Invoking(() => _ = _cache.GetOrCreateMapper(typeof(DummySource2), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("targetType");
            }

            [Fact]
            public void Should_Get_Existing_Mapper()
            {
                _cache.TryGetMapper(typeof(DummySource2), typeof(DummyTarget), out _).Should().BeFalse();

                var createdMapper = _cache.GetOrCreateMapper(typeof(DummySource2), typeof(DummyTarget));

                var actual = _cache.GetOrCreateMapper(typeof(DummySource2), typeof(DummyTarget));

                createdMapper.Should().BeSameAs(actual);
            }

            [Fact]
            public void Should_Create_Mapper_With_Default_Options()
            {
                _cache.TryGetMapper(typeof(DummySource2), typeof(DummyTarget), out _).Should().BeFalse();

                var actual = _cache.GetOrCreateMapper(typeof(DummySource2), typeof(DummyTarget));

                actual.Should().NotBeNull();
                actual.MatcherOptions.Should().BeSameAs(PropertyMatcherOptions.None);
            }
        }

        public class TryGetMapper : PropertyMatcherCacheFixture
        {
            private readonly PropertyMatcherCache _cache = new();

            [Fact]
            public void Should_Throw_When_Source_Type_Null()
            {
                Invoking(() => _ = _cache.TryGetMapper(null, typeof(DummyTarget), out var mapper))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceType");
            }

            [Fact]
            public void Should_Throw_When_Target_Type_Null()
            {
                Invoking(() => _ = _cache.TryGetMapper(typeof(DummySource2), null, out var mapper))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("targetType");
            }

            [Fact]
            public void Should_Get_Mapper_With_Default_Options()
            {
                _cache.TryGetMapper(typeof(DummySource2), typeof(DummyTarget), out _).Should().BeFalse();

                var matcherOptions = Create<PropertyMatcherOptions>();

                _ = _cache.CreateMapper(typeof(DummySource2), typeof(DummyTarget), matcherOptions);

                _cache.TryGetMapper(typeof(DummySource2), typeof(DummyTarget), out var actual).Should().BeTrue();

                actual.Should().NotBeNull();
                actual.MatcherOptions.Should().BeSameAs(matcherOptions);
            }
        }
    }
}