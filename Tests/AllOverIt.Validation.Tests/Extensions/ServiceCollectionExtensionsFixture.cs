using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

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
        }
    }
}