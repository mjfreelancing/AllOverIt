using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.Fixture;
using AllOverIt.Formatters.Objects;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Formatters.Objects
{
    public class ObjectPropertyFilterRegistryFixture : FixtureBase
    {
        private sealed class ObjectDummy
        {
        }

        private sealed class ObjectDummyFilter : ObjectPropertyFilter, IRegisteredObjectPropertyFilter
        {
            public bool CanFilter(object @object)
            {
                return @object is ObjectDummy;
            }
        }

        public class Register : ObjectPropertyFilterRegistryFixture
        {
            private readonly ObjectPropertyFilterRegistry _registry;

            public Register()
            {
                _registry = new ObjectPropertyFilterRegistry();
            }

            [Fact]
            public void Should_Register_Filter()
            {
                _registry.Register<ObjectDummyFilter>();

                var actual = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_With_Options()
            {
                var options = new ObjectPropertySerializerOptions();
                _registry.Register<ObjectDummyFilter>(options);

                _ = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer);

                serializer.Options.Should().BeSameAs(options);
            }
        }

        public class GetObjectPropertySerializer : ObjectPropertyFilterRegistryFixture
        {
            private readonly ObjectPropertyFilterRegistry _registry;

            public GetObjectPropertySerializer()
            {
                _registry = new ObjectPropertyFilterRegistry();
                _registry.Register<ObjectDummyFilter>();
            }

            [Fact]
            public void Should_Return_True_For_Registered_Filter()
            {
                var actual = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False_When_No_Suitable_Filter()
            {
                var actual = _registry.GetObjectPropertySerializer(Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Serializer_With_Filter()
            {
                var registry = new ObjectPropertyFilterRegistry();

                registry.Register<ObjectDummyFilter>();

                _ = registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer);

                serializer.Options.Filter.Should().BeOfType<ObjectDummyFilter>();
            }
        }
    }
}