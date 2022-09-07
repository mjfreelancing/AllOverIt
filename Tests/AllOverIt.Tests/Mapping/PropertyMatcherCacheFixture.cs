using AllOverIt.Fixture;
using AllOverIt.Mapping;
using AllOverIt.Mapping.Exceptions;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Reflection;
using Xunit;
using AllOverIt.Fixture.Extensions;

using static AllOverIt.Tests.Mapping.ObjectMapperTypes;

namespace AllOverIt.Tests.Mapping
{

    public class PropertyMatcherCacheFixture : FixtureBase
    {
        public class DefaultOptions : PropertyMatcherCacheFixture
        {
            [Fact]
            public void Should_Have_Default_Options()
            {
                var cache = new PropertyMatcherCache(null);

                var expected = new
                {
                    DeepCopy = false,
                    Binding = BindingOptions.Default,
                    Filter = (Func<PropertyInfo, bool>) null,
                    AllowNullCollections = false
                };

                expected
                    .Should()
                    .BeEquivalentTo(cache.DefaultOptions, opt => opt.IncludingInternalProperties());
            }

            [Fact]
            public void Should_Have_Provided_Options()
            {
                var expected = new PropertyMatcherOptions();
                var cache = new PropertyMatcherCache(expected);

                expected
                    .Should()
                    .BeSameAs(cache.DefaultOptions);
            }
        }

        public class CreateMapper : PropertyMatcherCacheFixture
        {
            private readonly PropertyMatcherCache _cache = new(null);

            [Fact]
            public void Should_Throw_When_Source_Type_Null()
            {
                Invoking(() =>_cache.CreateMapper(null, typeof(DummyTarget), Create<PropertyMatcherOptions>()))
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
        }

        public class GetOrCreateMapper : PropertyMatcherCacheFixture
        {
            private readonly PropertyMatcherCache _cache = new(null);

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
            public void Should_Create_Mapper()
            {
                _cache.TryGetMapper(typeof(DummySource2), typeof(DummyTarget), out _).Should().BeFalse();

                var actual = _cache.GetOrCreateMapper(typeof(DummySource2), typeof(DummyTarget));

                actual.Should().NotBeNull();
            }
        }

        public class TryGetMapper : PropertyMatcherCacheFixture
        {
            private readonly PropertyMatcherCache _cache = new(null);

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
        }
    }
}