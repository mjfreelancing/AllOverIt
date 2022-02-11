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

        private sealed class ObjectDummyFilter : ObjectPropertyFilter, IRegisteredObjectPropertyFilter
        {
            public bool CanFilter(object @object)
            {
                return @object is ObjectDummy;
            }
        }

        public class Register_Type : ObjectPropertyFilterRegistryFixture
        {
            private readonly ObjectPropertyFilterRegistry _registry;

            public Register_Type()
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

            [Fact]
            public void Should_Throw_If_Options_Registered_With_Filter()
            {
                var options = new ObjectPropertySerializerOptions
                {
                    Filter = new ObjectDummyFilter()
                };

                Invoking(() =>
                    {
                        _registry.Register<ObjectDummyFilter>(options);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage($"The {nameof(ObjectPropertyFilterRegistry)} expects the provided options to not include a filter. ({nameof(options.Filter)})");
            }
        }

        public class Register_Instance : ObjectPropertyFilterRegistryFixture
        {
            private readonly ObjectPropertyFilterRegistry _registry;

            public Register_Instance()
            {
                _registry = new ObjectPropertyFilterRegistry();
            }

            [Fact]
            public void Should_Register_Filter()
            {
                var filter = new ObjectDummyFilter();
                _registry.Register(filter);

                var actual = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_With_Options()
            {
                var filter = new ObjectDummyFilter();
                var options = new ObjectPropertySerializerOptions();
                _registry.Register(filter, options);

                _ = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer);

                serializer.Options.Should().BeSameAs(options);
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
                        var filter = new ObjectDummyFilter();
                        _registry.Register(filter, options);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage($"The {nameof(ObjectPropertyFilterRegistry)} expects the provided options to not include a filter. ({nameof(options.Filter)})");
            }
        }

        public class Register_Type_Action : ObjectPropertyFilterRegistryFixture
        {
            private readonly ObjectPropertyFilterRegistry _registry;

            public Register_Type_Action()
            {
                _registry = new ObjectPropertyFilterRegistry();
            }

            [Fact]
            public void Should_Register_Filter()
            {
                _registry.Register<ObjectDummyFilter>(options => { });

                var actual = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_With_Options()
            {
                var expectedOptions = new ObjectPropertySerializerOptions
                {
                    IncludeNulls = Create<bool>(),
                    IncludeEmptyCollections = Create<bool>(),
                    NullValueOutput = Create<string>(),
                    EmptyValueOutput = Create<string>()
                };

                _registry.Register<ObjectDummyFilter>(options =>
                {
                    options.IncludeNulls = expectedOptions.IncludeNulls;
                    options.IncludeEmptyCollections = expectedOptions.IncludeEmptyCollections;
                    options.NullValueOutput = expectedOptions.NullValueOutput;
                    options.EmptyValueOutput = expectedOptions.EmptyValueOutput;
                });

                _ = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer);

                serializer.Options.Filter.Should().BeOfType<ObjectDummyFilter>();
                serializer.Options.Should().BeEquivalentTo(expectedOptions, config => config.Excluding(options => options.Filter));
            }

            [Fact]
            public void Should_Throw_If_Options_Registered_With_Filter()
            {
                Invoking(() =>
                    {
                        _registry.Register<ObjectDummyFilter>(options =>
                        {
                            options.Filter = new ObjectDummyFilter();
                        });
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage($"The {nameof(ObjectPropertyFilterRegistry)} expects the provided options to not include a filter. ({nameof(ObjectPropertySerializerOptions.Filter)})");
            }
        }

        public class Register_Instance_Action : ObjectPropertyFilterRegistryFixture
        {
            private readonly ObjectPropertyFilterRegistry _registry;

            public Register_Instance_Action()
            {
                _registry = new ObjectPropertyFilterRegistry();
            }

            [Fact]
            public void Should_Register_Filter()
            {
                var filter = new ObjectDummyFilter();

                _registry.Register(filter, options => { });

                var actual = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_With_Options()
            {
                var expectedOptions = new ObjectPropertySerializerOptions
                {
                    IncludeNulls = Create<bool>(),
                    IncludeEmptyCollections = Create<bool>(),
                    NullValueOutput = Create<string>(),
                    EmptyValueOutput = Create<string>()
                };

                var filter = new ObjectDummyFilter();

                _registry.Register(filter, options =>
                {
                    options.IncludeNulls = expectedOptions.IncludeNulls;
                    options.IncludeEmptyCollections = expectedOptions.IncludeEmptyCollections;
                    options.NullValueOutput = expectedOptions.NullValueOutput;
                    options.EmptyValueOutput = expectedOptions.EmptyValueOutput;
                });

                _ = _registry.GetObjectPropertySerializer(Create<ObjectDummy>(), out var serializer);

                serializer.Options.Filter.Should().BeOfType<ObjectDummyFilter>();
                serializer.Options.Should().BeEquivalentTo(expectedOptions, config => config.Excluding(options => options.Filter));
            }

            [Fact]
            public void Should_Throw_If_Options_Registered_With_Filter()
            {
                Invoking(() =>
                {
                    var filter = new ObjectDummyFilter();

                    _registry.Register(filter, options =>
                    {
                        options.Filter = new ObjectDummyFilter();
                    });
                })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage($"The {nameof(ObjectPropertyFilterRegistry)} expects the provided options to not include a filter. ({nameof(ObjectPropertySerializerOptions.Filter)})");
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