using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Validation.Extensions;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Validation.Tests
{
    public class ValidationRegistryExtensionsFixture : FixtureBase
    {
        private sealed class DummyModel
        {
        }

        private sealed class DummyModelValidator : ValidatorBase<DummyModel>
        {
        }

        private class DummyRegistrar : ValidationRegistrarBase
        {
        }

        public class AutoRegisterValidators : ValidationRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ValidationRegistry_Null()
            {
                Invoking(() =>
                {
                    ValidationRegistryExtensions.AutoRegisterValidators<DummyRegistrar>(null);
                })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("validationRegistry");
            }

            [Fact]
            public void Should_Filter_Validators()
            {
                var wasFiltered = false;

                var invoker = new ValidationInvoker();

                ValidationRegistryExtensions.AutoRegisterValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    wasFiltered = wasFiltered || validatorType == typeof(DummyModelValidator);
                    return false;
                });

                wasFiltered.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_Validators()
            {
                var invoker = new ValidationInvoker();

                ValidationRegistryExtensions.AutoRegisterValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                // registering the validator a second time will throw an error

                Invoking(() =>
                {
                    invoker.Register<DummyModel, DummyModelValidator>();
                })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithMessage($"An item with the same key has already been added. Key: {typeof(DummyModel).FullName}");
            }
        }
    }
}
