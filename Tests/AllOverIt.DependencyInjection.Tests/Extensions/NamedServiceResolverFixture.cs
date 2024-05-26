using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Tests.Extensions
{
    public class NamedServiceResolverFixture : FixtureBase
    {
        private interface IDummyInterface
        {
        }

        private sealed class Dummy1 : IDummyInterface
        {
        }

        private sealed class Dummy2
        {
        }

        private sealed class Dummy3 : IDummyInterface
        {
        }

        private readonly NamedServiceResolver<IDummyInterface> _serviceResolver = new NamedServiceResolver<IDummyInterface>();
        private INamedServiceRegistration<IDummyInterface> _registration => _serviceResolver;
        private readonly IServiceProvider _serviceProvider;  // using a real one since it's easier than faking against extension methods

        public NamedServiceResolverFixture()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<IDummyInterface, Dummy1>();
            serviceCollection.AddTransient<IDummyInterface, Dummy3>();
            serviceCollection.AddSingleton<INamedServiceResolver<IDummyInterface>>(_serviceResolver);       // needed for NamedServiceResolver tests

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _serviceResolver._provider = _serviceProvider;
        }

        public class Generic : NamedServiceResolverFixture
        {
            public class Register_Typed : Generic
            {
                [Fact]
                public void Should_Throw_When_Name_Null_Empty_Whitespace()
                {
                    AssertThrowsWhenStringNullOrEmptyOrWhitespace(stringValue =>
                    {
                        _registration.Register<Dummy1>(stringValue);
                    }, "name");
                }

                [Fact]
                public void Should_Register_Named_Service()
                {
                    _serviceResolver._provider = _serviceProvider;

                    var name = Create<string>();

                    _registration.Register<Dummy1>(name);

                    var actual = ((INamedServiceResolver<IDummyInterface>) _serviceResolver).GetRequiredNamedService(name);

                    actual.Should().BeOfType<Dummy1>();
                }

                [Fact]
                public void Should_Throw_When_Name_Is_Duplicate()
                {
                    var name = Create<string>();

                    _registration.Register<Dummy1>(name);

                    Invoking(() =>
                    {
                        _registration.Register<Dummy1>(name);
                    })
                    .Should()
                    .Throw<DependencyRegistrationException>()
                    .WithMessage($"The name '{name}' has already been registered against the type '{nameof(Dummy1)}'.");
                }
            }

            public class Register_Type : Generic
            {
                [Fact]
                public void Should_Throw_When_Name_Null_Empty_Whitespace()
                {
                    AssertThrowsWhenStringNullOrEmptyOrWhitespace(stringValue =>
                    {
                        _registration.Register(stringValue, typeof(Dummy1));
                    }, "name");
                }

                [Fact]
                public void Should_Register_Named_Service()
                {
                    _serviceResolver._provider = _serviceProvider;

                    var name = Create<string>();

                    _registration.Register(name, typeof(Dummy1));

                    var actual = ((INamedServiceResolver<IDummyInterface>) _serviceResolver).GetRequiredNamedService(name);

                    actual.Should().BeOfType<Dummy1>();
                }

                [Fact]
                public void Should_Throw_When_Type_Does_Not_Implement_Service()
                {
                    var name = Create<string>();

                    Invoking(() =>
                    {
                        _registration.Register(name, typeof(Dummy2));
                    })
                    .Should()
                    .Throw<DependencyRegistrationException>()
                    .WithMessage($"The type '{typeof(Dummy2).GetFriendlyName()}' with name '{name}' does not implement '{nameof(IDummyInterface)}'.");
                }

                [Fact]
                public void Should_Throw_When_Name_Is_Duplicate()
                {
                    var name = Create<string>();

                    _registration.Register(name, typeof(Dummy1));

                    Invoking(() =>
                    {
                        _registration.Register(name, typeof(Dummy1));
                    })
                    .Should()
                    .Throw<DependencyRegistrationException>()
                    .WithMessage($"The name '{name}' has already been registered against the type '{nameof(Dummy1)}'.");
                }
            }

            public class GetRequiredNamedService : Generic
            {
                [Fact]
                public void Should_Throw_When_Name_Null_Empty_Whitespace()
                {
                    AssertThrowsWhenStringNullOrEmptyOrWhitespace(stringValue =>
                    {
                        _ = ((INamedServiceResolver<IDummyInterface>) _serviceResolver).GetRequiredNamedService(stringValue);
                    }, "name");
                }

                [Fact]
                public void Should_Resolve_Service()
                {
                    var name = Create<string>();

                    _registration.Register(name, typeof(Dummy1));

                    var actual = ((INamedServiceResolver<IDummyInterface>) _serviceResolver).GetRequiredNamedService(name);

                    actual.Should().BeOfType<Dummy1>();
                }

                [Fact]
                public void Should_Throw_When_Service_Not_Registered()
                {
                    var name = Create<string>();

                    Invoking(() =>
                    {
                        _ = ((INamedServiceResolver<IDummyInterface>) _serviceResolver).GetRequiredNamedService(name);
                    })
                    .Should()
                    .Throw<DependencyRegistrationException>()
                    .WithMessage($"No service of type {nameof(IDummyInterface)} was found for the name {name}.");
                }

                [Fact]
                public void Should_Register_Multiple_Names_To_Same_Implementation()
                {
                    var name1 = Create<string>();
                    var name2 = Create<string>();
                    var name3 = Create<string>();

                    _registration.Register(name1, typeof(Dummy1));
                    _registration.Register(name2, typeof(Dummy1));
                    _registration.Register(name3, typeof(Dummy3));

                    var actual1 = ((INamedServiceResolver<IDummyInterface>) _serviceResolver).GetRequiredNamedService(name1);
                    var actual2 = ((INamedServiceResolver<IDummyInterface>) _serviceResolver).GetRequiredNamedService(name2);
                    var actual3 = ((INamedServiceResolver<IDummyInterface>) _serviceResolver).GetRequiredNamedService(name3);

                    actual1.Should().BeOfType<Dummy1>();
                    actual2.Should().BeOfType<Dummy1>();
                    actual3.Should().BeOfType<Dummy3>();
                }
            }
        }

        public class NonGeneric : NamedServiceResolverFixture
        {
            public class Constructor : NonGeneric
            {

                [Fact]
                public void Should_Throw_When_Provider_Null()
                {

                    Invoking(() =>
                    {
                        _ = new NamedServiceResolver(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("provider");
                }
            }

            public class GetRequiredNamedService : NonGeneric
            {
                private readonly NamedServiceResolver _resolver;

                public GetRequiredNamedService()
                {
                    _resolver = new NamedServiceResolver(_serviceProvider);
                }

                [Fact]
                public void Should_Throw_When_Name_Null_Empty_Whitespace()
                {
                    AssertThrowsWhenStringNullOrEmptyOrWhitespace(stringValue =>
                    {
                        _ = ((INamedServiceResolver) _resolver).GetRequiredNamedService<IDummyInterface>(stringValue);
                    }, "name");
                }

                [Fact]
                public void Should_Resolve_Service()
                {
                    var name = Create<string>();

                    _registration.Register(name, typeof(Dummy1));

                    // Test setup includes registration of INamedServiceResolver<IDummyInterface>
                    var actual = ((INamedServiceResolver) _resolver).GetRequiredNamedService<IDummyInterface>(name);

                    actual.Should().BeOfType<Dummy1>();
                }

                [Fact]
                public void Should_Throw_When_Service_Not_Registered()
                {
                    var name = Create<string>();

                    Invoking(() =>
                    {
                        // Test setup includes registration of INamedServiceResolver<IDummyInterface>
                        _ = ((INamedServiceResolver) _resolver).GetRequiredNamedService<IDummyInterface>(name);
                    })
                    .Should()
                    .Throw<DependencyRegistrationException>()
                    .WithMessage($"No service of type {nameof(IDummyInterface)} was found for the name {name}.");
                }
            }
        }
    }
}