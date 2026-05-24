using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Validation.Extensions;
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
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddValidationInvoker(null, null);
                });
                exception.WithNamedMessageWhenNull("services");
            }

            [Fact]
            public void Should_Invoke_Configure_Registry()
            {
                IValidationRegistry validationRegistry = null;

                _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddValidationInvoker(_services, registry =>
                {
                    validationRegistry = registry;
                });

                validationRegistry.ShouldNotBeNull();
            }

            [Fact]
            public void Should_Register_Invoker_As_Singleton()
            {
                _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddValidationInvoker(_services);

                _services
                    .Where(descriptor => descriptor.ServiceType == typeof(IValidationInvoker))
                    .SingleOrDefault()
                    .Lifetime
                     .ShouldBe(ServiceLifetime.Singleton);
            }

            [Fact]
            public void Should_Return_Registry_As_Invoker()
            {
                var actual = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddValidationInvoker(_services);

                actual.ShouldBeOfType(typeof(ValidationInvoker));

                actual.GetType()
                    .IsDerivedFrom(typeof(IValidationRegistry))
                    .ShouldBeTrue();
            }
        }

        public class AddLifetimeValidationInvoker : ServiceCollectionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ServiceCollection_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddLifetimeValidationInvoker(null, null);
                });
                exception.WithNamedMessageWhenNull("services");
            }

            [Fact]
            public void Should_Invoke_Configure_Registry()
            {
                ILifetimeValidationRegistry validationRegistry = null;

                _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddLifetimeValidationInvoker(_services, registry =>
                {
                    validationRegistry = registry;
                });

                validationRegistry.ShouldNotBeNull();
            }

            [Fact]
            public void Should_Register_Invoker_As_Singleton()
            {
                _ = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddLifetimeValidationInvoker(_services);

                _services
                    .Where(descriptor => descriptor.ServiceType == typeof(ILifetimeValidationInvoker))
                    .SingleOrDefault()
                    .Lifetime
                     .ShouldBe(ServiceLifetime.Singleton);
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

                Should.NotThrow(() =>
                {
                    // This would throw if the provider was not set
                    invoker.Validate<DummyModel>(Create<DummyModel>());
                });
            }

            [Fact]
            public void Should_Return_Registry_As_Invoker()
            {
                var actual = AllOverIt.Validation.Extensions.ServiceCollectionExtensions.AddLifetimeValidationInvoker(_services);

                actual.ShouldBeOfType(typeof(LifetimeValidationInvoker));

                actual.GetType()
                    .IsDerivedFrom(typeof(ILifetimeValidationRegistry))
                    .ShouldBeTrue();
            }

            [Fact]
            public void Should_Resolve_Transient_Validator()
            {
                _services.AddLifetimeValidationInvoker(registry =>
                {
                    registry.RegisterTransient<DummyModel, DummyModelValidator>();
                });

                Should.NotThrow(() =>
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
                });
            }

            [Fact]
            public void Should_Resolve_Scoped_Validator()
            {
                _services.AddLifetimeValidationInvoker(registry =>
                {
                    registry.RegisterScoped<DummyModel, DummyModelValidator>();
                });

                Should.NotThrow(() =>
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
                });
            }

            [Fact]
            public void Should_Resolve_Singleton_Validator()
            {
                _services.AddLifetimeValidationInvoker(registry =>
                {
                    registry.RegisterSingleton<DummyModel, DummyModelValidator>();
                });

                Should.NotThrow(() =>
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
                });
            }
        }
    }
}





