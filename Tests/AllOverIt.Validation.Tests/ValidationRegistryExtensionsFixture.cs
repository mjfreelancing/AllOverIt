using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Validation.Exceptions;
using AllOverIt.Validation.Extensions;
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
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    ValidationRegistryExtensions.AutoRegisterValidators<DummyRegistrar>(null);
                });

                exception.WithNamedMessageWhenNull("validationRegistry");
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

                wasFiltered.ShouldBeTrue();
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

                validator.ShouldBeOfType(typeof(DummyModelValidator));
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
                var exception = Should.Throw<ValidationRegistryException>(() =>
                {
                    ((IValidationRegistry) invoker).Register<DummyModel, DummyModelValidator>();
                });

                exception.Message.ShouldBe($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }
        }
    }
}






