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
                expected.Filter = serializer.Options.Filter;

                serializer.Options.Should().BeEquivalentTo(expected);
                serializer.Options.Filter.Should().BeOfType<ObjectDummyFilter>();
            }

            [Fact]
            public void Should_Register_With_Provided_Options_And_Filter()
            {
                var options = new ObjectPropertySerializerOptions();
                _registry.Register<ObjectDummy, ObjectDummyFilter>(options);

                _ = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer);

                serializer.Options.Should().BeSameAs(options);
                serializer.Options.Filter.Should().BeOfType<ObjectDummyFilter>();
            }

            [Fact]
            public void Should_Throw_If_Options_Registered_With_Filter()
            {
                var options = new ObjectPropertySerializerOptions
                {
                    Filter = new ObjectDummyFilter()
                };

                Invoking(() =>
                    {
                        _registry.Register<ObjectDummy, ObjectDummyFilter>(options);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage($"The {nameof(ObjectPropertyFilterRegistry)} expects the provided options to not include a filter. ({nameof(options.Filter)})");
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

                serializer.Options.Should().BeEquivalentTo(expectedOptions, config => config.Excluding(options => options.Filter));
                serializer.Options.Filter.Should().BeOfType<ObjectDummyFilter>();
            }

            [Fact]
            public void Should_Throw_If_Options_Registered_With_Filter()
            {
                Invoking(() =>
                    {
                        _registry.Register<ObjectDummy, ObjectDummyFilter>(options =>
                        {
                            options.Filter = new ObjectDummyFilter();
                        });

                        // need to get a serializer to force the options to be invoked
                        _ = _registry.GetObjectPropertySerializer<ObjectDummy>(out var serializer);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage($"The {nameof(ObjectPropertyFilterRegistry)} expects the provided options to not include a filter. ({nameof(ObjectPropertySerializerOptions.Filter)})");
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

                serializer.Options.Filter.Should().BeOfType<ObjectDummyFilter>();
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

                serializer.Options.Filter.Should().BeOfType<ObjectDummyFilter>();
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

                    serializer.Options.Filter.Should().BeOfType<ObjectDummyFilter>();
                }
            }
        }
    }
}