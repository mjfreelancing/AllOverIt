using AllOverIt.Fixture;
using AllOverIt.Formatters.Objects;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Formatters.Objects
{
    public class ObjectPropertyFilterRegistryFixture : FixtureBase
    {
        private sealed class ObjectDummy
        {
        }

        private sealed class ObjectDummyFilter : ObjectPropertyFilter
        {
        }

        public class Register_With_Options : ObjectPropertyFilterRegistryFixture
        {
            private readonly ObjectPropertyFilterRegistry _registry;

            public Register_With_Options()
            {
                _registry = new ObjectPropertyFilterRegistry();
            }

            [Fact]
            public void Should_Register_Filter()
            {
                _registry.Register<ObjectDummy, ObjectDummyFilter>();

                var actual = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_Filter_With_Default_Options_And_Filter()
            {
                _registry.Register<ObjectDummy, ObjectDummyFilter>();

                _ = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer);

                var expected = new ObjectPropertySerializerOptions();

                serializer.Options.Should().BeEquivalentTo(expected);
                serializer.Filter.Should().BeOfType<ObjectDummyFilter>();
            }

            [Fact]
            public void Should_Create_Shared_Options_And_New_Filter_Per_Serializer()
            {
                _registry.Register<ObjectDummy, ObjectDummyFilter>();

                var actual1 = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer1);
                var actual2 = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer2);

                actual1.Should().BeTrue();
                actual2.Should().BeTrue();

                var expected = new ObjectPropertySerializerOptions();

                serializer1.Options.Should().BeEquivalentTo(expected);
                serializer2.Options.Should().BeEquivalentTo(expected);

                serializer1.Options.Should().BeSameAs(serializer2.Options);
                serializer1.Filter.Should().NotBeSameAs(serializer2.Filter);
            }

            [Fact]
            public void Should_Register_With_Provided_Options_And_Filter()
            {
                var options = new ObjectPropertySerializerOptions();
                _registry.Register<ObjectDummy, ObjectDummyFilter>(options);

                _ = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer);

                serializer.Options.Should().BeSameAs(options);
                serializer.Filter.Should().BeOfType<ObjectDummyFilter>();
            }
        }

        public class Register_With_Options_Action : ObjectPropertyFilterRegistryFixture
        {
            private readonly ObjectPropertyFilterRegistry _registry;

            public Register_With_Options_Action()
            {
                _registry = new ObjectPropertyFilterRegistry();
            }

            [Fact]
            public void Should_Register_Filter()
            {
                _registry.Register<ObjectDummy, ObjectDummyFilter>(options => { });

                var actual = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_New_Options_And_New_Filter_Per_Serializer()
            {
                _registry.Register<ObjectDummy, ObjectDummyFilter>(options => { });

                var actual1 = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer1);
                var actual2 = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer2);

                actual1.Should().BeTrue();
                actual2.Should().BeTrue();

                serializer1.Options.Should().NotBeSameAs(serializer2.Options);
                serializer1.Filter.Should().NotBeSameAs(serializer2.Filter);
            }

            [Fact]
            public void Should_Register_With_Provided_Options_And_Filter()
            {
                var expectedOptions = new ObjectPropertySerializerOptions
                {
                    IncludeNulls = Create<bool>(),
                    IncludeEmptyCollections = Create<bool>(),
                    NullValueOutput = Create<string>(),
                    EmptyValueOutput = Create<string>()
                };

                _registry.Register<ObjectDummy, ObjectDummyFilter>(options =>
                {
                    options.IncludeNulls = expectedOptions.IncludeNulls;
                    options.IncludeEmptyCollections = expectedOptions.IncludeEmptyCollections;
                    options.NullValueOutput = expectedOptions.NullValueOutput;
                    options.EmptyValueOutput = expectedOptions.EmptyValueOutput;
                });

                _ = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer);

                serializer.Filter.Should().BeOfType<ObjectDummyFilter>();
            }
        }

        public class GetObjectPropertySerializer_Object : ObjectPropertyFilterRegistryFixture
        {
            private readonly ObjectPropertyFilterRegistry _registry;

            public GetObjectPropertySerializer_Object()
            {
                _registry = new ObjectPropertyFilterRegistry();
                _registry.Register<ObjectDummy, ObjectDummyFilter>();
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

                registry.Register<ObjectDummy, ObjectDummyFilter>();

                _ = registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer);

                serializer.Filter.Should().BeOfType<ObjectDummyFilter>();
            }
        }

        public class GetObjectPropertySerializer_Generic_Type : ObjectPropertyFilterRegistryFixture
        {
            private readonly ObjectPropertyFilterRegistry _registry;

            public GetObjectPropertySerializer_Generic_Type()
            {
                _registry = new ObjectPropertyFilterRegistry();
                _registry.Register<ObjectDummy, ObjectDummyFilter>();
            }

            [Fact]
            public void Should_Return_True_For_Registered_Filter()
            {
                var actual = _registry.GetObjectPropertySerializer<ObjectDummy>(out _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False_When_No_Suitable_Filter()
            {
                var actual = _registry.GetObjectPropertySerializer<string>(out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Serializer_With_Filter()
            {
                var registry = new ObjectPropertyFilterRegistry();

                registry.Register<ObjectDummy, ObjectDummyFilter>();

                _ = registry.GetObjectPropertySerializer<ObjectDummy>(out var serializer);

                serializer.Filter.Should().BeOfType<ObjectDummyFilter>();
            }
        }

        public class GetObjectPropertySerializer_Type : ObjectPropertyFilterRegistryFixture
        {
            private readonly ObjectPropertyFilterRegistry _registry;

            public GetObjectPropertySerializer_Type()
            {
                _registry = new ObjectPropertyFilterRegistry();
                _registry.Register<ObjectDummy, ObjectDummyFilter>();
            }

            [Fact]
            public void Should_Return_True_For_Registered_Filter()
            {
                var actual = _registry.GetObjectPropertySerializer(typeof(ObjectDummy), out _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False_When_No_Suitable_Filter()
            {
                var actual = _registry.GetObjectPropertySerializer(typeof(string), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Serializer_With_Filter()
            {
                var registry = new ObjectPropertyFilterRegistry();

                registry.Register<ObjectDummy, ObjectDummyFilter>();

                _ = registry.GetObjectPropertySerializer(typeof(ObjectDummy), out var serializer);

                serializer.Filter.Should().BeOfType<ObjectDummyFilter>();
            }
        }
    }
}