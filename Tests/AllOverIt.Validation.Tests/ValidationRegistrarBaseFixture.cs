using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Validation.Exceptions;
using FakeItEasy;

namespace AllOverIt.Validation.Tests
{

    public class ValidationRegistrarBaseFixture : FixtureBase
    {
        private sealed class DummyModel
        {
        }

        private abstract class DummyAbstractModelValidator : ValidatorBase<DummyModel>
        {
        }

        // Deliberately has a base class without a generic
        private sealed class DummyModelValidator : DummyAbstractModelValidator
        {
        }

        private class DummyRegistrar : ValidationRegistrarBase
        {
        }

        private readonly ValidationRegistrarBase _validationRegistrar = new DummyRegistrar();

        public class AutoRegisterValidators : ValidationRegistrarBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    _validationRegistrar.AutoRegisterValidators(null);
                });

                exception.WithNamedMessageWhenNull("validationRegistry");
            }

            [Fact]
            public void Should_Filter_Validators()
            {
                var wasFiltered = false;

                var invoker = new ValidationInvoker();

                _validationRegistrar.AutoRegisterValidators(invoker, (modelType, validatorType) =>
                {
                    wasFiltered = wasFiltered || validatorType == typeof(DummyModelValidator);
                    return false;
                });

                wasFiltered.ShouldBeTrue();
            }

            [Fact]
            public void Should_Register_All_Validators_When_Predicate_Null()
            {
                var validators = new List<Type>();

                // Cannot use a ValidationInvoker in this test since not all have a default ctor (for another test)
                var registryFake = this.CreateFake<IValidationRegistry>();

                registryFake
                    .CallsTo(fake => fake.Register(A<Type>.Ignored, A<Type>.Ignored))
                    .Invokes(call =>
                    {
                        var validatorType = (Type)call.Arguments[1];
                        validators.Add(validatorType);
                    });

                _validationRegistrar.AutoRegisterValidators(registryFake.FakedObject, null);

                validators.Count.ShouldBe(32);      // All non-abstract validators in this assembly

                validators.All(validator => !validator.IsAbstract).ShouldBeTrue();
            }

            [Fact]
            public void Should_Throw_When_Validator_Already_Registered()
            {
                var invoker = new ValidationInvoker();

                _validationRegistrar.AutoRegisterValidators(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                // registering the validator a second time will throw an error
                var exception = Should.Throw<ValidationRegistryException>(() =>
                {
                    ((IValidationRegistry)invoker).Register<DummyModel, DummyModelValidator>();
                });

                exception.Message.ShouldBe($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }
        }
    }
}






