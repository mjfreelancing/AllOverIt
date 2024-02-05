using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Validation.Exceptions;
using AllOverIt.Validation.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace AllOverIt.Validation.Tests
{
    public class LifetimeValidationRegistryExtensionsFixture : FixtureBase
    {
        private sealed class DummyModel
        {
        }

        private sealed class DummyModelValidator : ValidatorBase<DummyModel>
        {
        }

        private class DummyRegistrar : LifetimeValidationRegistrarBase
        {
        }

        private readonly IServiceCollection _services = new ServiceCollection();

        public class AutoRegisterTransientValidators : LifetimeValidationRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ValidationRegistry_Null()
            {
                Invoking(() =>
                {
                    LifetimeValidationRegistryExtensions.AutoRegisterTransientValidators<DummyRegistrar>(null);
                })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("validationRegistry");
            }

            [Fact]
            public void Should_Filter_Validators()
            {
                var wasFiltered = false;

                var invoker = new LifetimeValidationInvoker(_services);

                LifetimeValidationRegistryExtensions.AutoRegisterTransientValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    wasFiltered = wasFiltered || validatorType == typeof(DummyModelValidator);
                    return false;
                });

                wasFiltered.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_Validators()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                LifetimeValidationRegistryExtensions.AutoRegisterTransientValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                // registering the validator a second time will throw an error

                Invoking(() =>
                {
                    invoker.RegisterTransient<DummyModel, DummyModelValidator>();
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }
        }



    }
}
