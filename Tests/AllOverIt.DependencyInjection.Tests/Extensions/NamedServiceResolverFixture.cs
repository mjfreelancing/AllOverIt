﻿using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.DependencyInjection.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

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

        private readonly NamedServiceResolver<IDummyInterface> _serviceResolver = new NamedServiceResolver<IDummyInterface>();
        private INamedServiceRegistration<IDummyInterface> _registration => _serviceResolver;
        private readonly IServiceProvider _serviceProvider;  // using a real one since it's easier than faking against extension methods

        public NamedServiceResolverFixture()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<IDummyInterface, Dummy1>();
            serviceCollection.AddSingleton<INamedServiceResolver<IDummyInterface>>(_serviceResolver);       // needed for NamedServiceResolver tests

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _serviceResolver._provider = _serviceProvider;
        }

        public class Generic : NamedServiceResolverFixture
        {
            public class Register_Typed : Generic
            {
                [Fact]
                public void Should_Throw_When_Name_Null()
                {
                    Invoking(() =>
                    {
                        _registration.Register<Dummy1>(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
                }

                [Fact]
                public void Should_Throw_When_Name_Empty()
                {
                    Invoking(() =>
                    {
                        _registration.Register<Dummy1>(string.Empty);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
                }

                [Fact]
                public void Should_Throw_When_Name_Whitespace()
                {
                    Invoking(() =>
                    {
                        _registration.Register<Dummy1>("  ");
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
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
                public void Should_Throw_When_Name_Null()
                {
                    Invoking(() =>
                    {
                        _registration.Register(null, typeof(Dummy1));
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
                }

                [Fact]
                public void Should_Throw_When_Name_Empty()
                {
                    Invoking(() =>
                    {
                        _registration.Register(string.Empty, typeof(Dummy1));
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
                }

                [Fact]
                public void Should_Throw_When_Name_Whitespace()
                {
                    Invoking(() =>
                    {
                        _registration.Register("  ", typeof(Dummy1));
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
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
                public void Should_Throw_When_Name_Null()
                {
                    Invoking(() =>
                    {
                        _ = ((INamedServiceResolver<IDummyInterface>) _serviceResolver).GetRequiredNamedService(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
                }

                [Fact]
                public void Should_Throw_When_Name_Empty()
                {
                    Invoking(() =>
                    {
                        _ = ((INamedServiceResolver<IDummyInterface>) _serviceResolver).GetRequiredNamedService(string.Empty);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
                }

                [Fact]
                public void Should_Throw_When_Name_Whitespace()
                {
                    Invoking(() =>
                    {
                        _ = ((INamedServiceResolver<IDummyInterface>) _serviceResolver).GetRequiredNamedService("  ");
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
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
                public void Should_Throw_When_Name_Null()
                {
                    Invoking(() =>
                    {
                        _ = ((INamedServiceResolver) _resolver).GetRequiredNamedService<IDummyInterface>(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
                }

                [Fact]
                public void Should_Throw_When_Name_Empty()
                {
                    Invoking(() =>
                    {
                        _ = ((INamedServiceResolver) _resolver).GetRequiredNamedService<IDummyInterface>(string.Empty);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
                }

                [Fact]
                public void Should_Throw_When_Name_Whitespace()
                {
                    Invoking(() =>
                    {
                        _ = ((INamedServiceResolver) _resolver).GetRequiredNamedService<IDummyInterface>("  ");
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
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