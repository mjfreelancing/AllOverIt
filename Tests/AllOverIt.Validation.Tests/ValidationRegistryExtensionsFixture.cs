using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Validation.Exceptions;
using AllOverIt.Validation.Extensions;
using FluentAssertions;
using FluentValidation;

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
            public void Should_Filter_Validator()
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
            public void Should_Register_Validator()
            {
                var validatorCache = new Dictionary<Type, Lazy<IValidator>>();
                var invoker = new ValidationInvoker(validatorCache);

                ValidationRegistryExtensions.AutoRegisterValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                var validator = validatorCache[typeof(DummyModel)].Value;

                validator.Should().BeOfType(typeof(DummyModelValidator));
            }

            [Fact]
            public void Should_Throw_When_Already_Registered()
            {
                var invoker = new ValidationInvoker();

                ValidationRegistryExtensions.AutoRegisterValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                // registering the validator a second time will throw an error
                Invoking(() =>
                {
                    ((IValidationRegistry) invoker).Register<DummyModel, DummyModelValidator>();
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }
        }
    }
}
