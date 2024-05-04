using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Validation.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Validation.Tests.Extensions
{
    public class ServiceCollectionExtensionsFixture : FixtureBase
    {
        private class DummyModel
        {
            public int? ValueOne { get; set; }
        }

        private class DummyModelValidator : ValidatorBase<DummyModel>
        {
        }

        private readonly IServiceCollection _services = new ServiceCollection();

        public class AddValidationInvoker : ServiceCollectionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ServiceCollection_Null()
            {
                Invoking(() =>
                {
                    _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddValidationInvoker(null, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("services");
            }

            [Fact]
            public void Should_Invoke_Configure_Registry()
            {
                IValidationRegistry validationRegistry = null;

                _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddValidationInvoker(_services, registry =>
                {
                    validationRegistry = registry;
                });

                validationRegistry.Should().NotBeNull();
            }

            [Fact]
            public void Should_Register_Invoker_As_Singleton()
            {
                _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddValidationInvoker(_services);

                _services
                    .Where(descriptor => descriptor.ServiceType == typeof(IValidationInvoker))
                    .SingleOrDefault()
                    .Lifetime
                    .Should()
                    .Be(ServiceLifetime.Singleton);
            }

            [Fact]
            public void Should_Return_Registry_As_Invoker()
            {
                var actual = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddValidationInvoker(_services);

                actual.Should().BeOfType(typeof(ValidationInvoker));

                actual.GetType()
                    .IsDerivedFrom(typeof(IValidationRegistry))
                    .Should()
                    .BeTrue();
            }
        }

        public class AddLifetimeValidationInvoker : ServiceCollectionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ServiceCollection_Null()
            {
                Invoking(() =>
                {
                    _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddLifetimeValidationInvoker(null, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("services");
            }

            [Fact]
            public void Should_Invoke_Configure_Registry()
            {
                ILifetimeValidationRegistry validationRegistry = null;

                _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddLifetimeValidationInvoker(_services, registry =>
                {
                    validationRegistry = registry;
                });

                validationRegistry.Should().NotBeNull();
            }

            [Fact]
            public void Should_Register_Invoker_As_Singleton()
            {
                _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddLifetimeValidationInvoker(_services);

                _services
                    .Where(descriptor => descriptor.ServiceType == typeof(ILifetimeValidationInvoker))
                    .SingleOrDefault()
                    .Lifetime
                    .Should()
                    .Be(ServiceLifetime.Singleton);
            }

            [Fact]
            public void Should_Set_Provider()
            {
                _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddLifetimeValidationInvoker(_services, registry =>
                {
                    registry.Register<DummyModel, DummyModelValidator>(Create<ServiceLifetime>());
                });

                var provider = _services.BuildServiceProvider();

                var invoker = provider.GetRequiredService<ILifetimeValidationInvoker>();

                Invoking(() =>
                {
                    // This would throw if the provider was not set
                    invoker.Validate<DummyModel>(Create<DummyModel>());
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Return_Registry_As_Invoker()
            {
                var actual = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddLifetimeValidationInvoker(_services);

                actual.Should().BeOfType(typeof(LifetimeValidationInvoker));

                actual.GetType()
                    .IsDerivedFrom(typeof(ILifetimeValidationRegistry))
                    .Should()
                    .BeTrue();
            }

            [Fact]
            public void Should_Resolve_Transient_Validator()
            {
                _services.AddLifetimeValidationInvoker(registry =>
                {
                    registry.RegisterTransient<DummyModel, DummyModelValidator>();
                });

                Invoking(() =>
                {
                    // Make sure the registrations are all valid (such as cannot resolve a
                    // scoped/transient within the singleton invoker).
                    var provider = _services.BuildServiceProvider(true);

                    using (var scope = provider.CreateScope())
                    {
                        var model = new DummyModel();

                        scope.ServiceProvider
                            .GetRequiredService<ILifetimeValidationInvoker>()
                            .AssertValidation(model);
                    }
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Resolve_Scoped_Validator()
            {
                _services.AddLifetimeValidationInvoker(registry =>
                {
                    registry.RegisterScoped<DummyModel, DummyModelValidator>();
                });

                Invoking(() =>
                {
                    // Make sure the registrations are all valid (such as cannot resolve a
                    // scoped/transient within the singleton invoker).
                    var provider = _services.BuildServiceProvider(true);

                    using (var scope = provider.CreateScope())
                    {
                        var model = new DummyModel();

                        scope.ServiceProvider
                            .GetRequiredService<ILifetimeValidationInvoker>()
                            .AssertValidation(model);
                    }
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Resolve_Singleton_Validator()
            {
                _services.AddLifetimeValidationInvoker(registry =>
                {
                    registry.RegisterSingleton<DummyModel, DummyModelValidator>();
                });

                Invoking(() =>
                {
                    // Make sure the registrations are all valid (such as cannot resolve a
                    // scoped/transient within the singleton invoker).
                    var provider = _services.BuildServiceProvider(true);

                    using (var scope = provider.CreateScope())
                    {
                        var model = new DummyModel();

                        scope.ServiceProvider
                            .GetRequiredService<ILifetimeValidationInvoker>()
                            .AssertValidation(model);
                    }
                })
                .Should()
                .NotThrow();
            }
        }
    }
}