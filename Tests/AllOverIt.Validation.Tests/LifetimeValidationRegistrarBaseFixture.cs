using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Validation.Exceptions;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AllOverIt.Validation.Tests
{
    public class LifetimeValidationRegistrarBaseFixture : FixtureBase
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

        private class DummyRegistrar : LifetimeValidationRegistrarBase
        {
        }

        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly LifetimeValidationRegistrarBase _validationRegistrar = new DummyRegistrar();

        public class AutoRegisterTransientValidators : LifetimeValidationRegistrarBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() =>
                {
                    _validationRegistrar.AutoRegisterTransientValidators(null);
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

                _validationRegistrar.AutoRegisterTransientValidators(invoker, (modelType, validatorType) =>
                {
                    wasFiltered = wasFiltered || validatorType == typeof(DummyModelValidator);
                    return false;
                });

                wasFiltered.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_All_Validators_When_Predicate_Null()
            {
                var validators = new List<Type>();

                // Cannot use a ValidationInvoker in this test since not all have a default ctor (for another test)
                var registryFake = this.CreateFake<ILifetimeValidationRegistry>();

                registryFake
                    .CallsTo(fake => fake.Register(A<Type>.Ignored, A<Type>.Ignored, A<ServiceLifetime>.Ignored))
                    .Invokes(call =>
                    {
                        var validatorType = (Type) call.Arguments[1];
                        validators.Add(validatorType);
                    });

                _validationRegistrar.AutoRegisterTransientValidators(registryFake.FakedObject, null);

                validators.Should().HaveCount(30);      // All non-abstract validators in this assembly

                validators.All(validator => !validator.IsAbstract).Should().BeTrue();
            }

            [Fact]
            public void Should_Throw_When_Validator_Already_Registered()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                _validationRegistrar.AutoRegisterTransientValidators(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                // registering the validator a second time will throw an error
                Invoking(() =>
                {
                    ((ILifetimeValidationRegistry) invoker).Register<DummyModel, DummyModelValidator>(Create<ServiceLifetime>());
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }

            [Fact]
            public void Should_Register_Validator_With_Lifetime()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                _validationRegistrar.AutoRegisterTransientValidators(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                _services
                    .Where(descriptor => descriptor.ServiceType == LifetimeValidationInvoker.CreateModelValidatorKey(typeof(DummyModel)))
                    .SingleOrDefault()
                    .Lifetime
                    .Should()
                    .Be(ServiceLifetime.Transient);
            }
        }

        public class AutoRegisterScopedValidators : LifetimeValidationRegistrarBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() =>
                {
                    _validationRegistrar.AutoRegisterScopedValidators(null);
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

                _validationRegistrar.AutoRegisterScopedValidators(invoker, (modelType, validatorType) =>
                {
                    wasFiltered = wasFiltered || validatorType == typeof(DummyModelValidator);
                    return false;
                });

                wasFiltered.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_All_Validators_When_Predicate_Null()
            {
                var validators = new List<Type>();

                // Cannot use a ValidationInvoker in this test since not all have a default ctor (for another test)
                var registryFake = this.CreateFake<ILifetimeValidationRegistry>();

                registryFake
                    .CallsTo(fake => fake.Register(A<Type>.Ignored, A<Type>.Ignored, A<ServiceLifetime>.Ignored))
                    .Invokes(call =>
                    {
                        var validatorType = (Type) call.Arguments[1];
                        validators.Add(validatorType);
                    });

                _validationRegistrar.AutoRegisterScopedValidators(registryFake.FakedObject, null);

                validators.Should().HaveCount(30);      // All non-abstract validators in this assembly

                validators.All(validator => !validator.IsAbstract).Should().BeTrue();
            }

            [Fact]
            public void Should_Throw_When_Validator_Already_Registered()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                _validationRegistrar.AutoRegisterScopedValidators(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                // registering the validator a second time will throw an error
                Invoking(() =>
                {
                    ((ILifetimeValidationRegistry) invoker).Register<DummyModel, DummyModelValidator>(Create<ServiceLifetime>());
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }

            [Fact]
            public void Should_Register_Validator_With_Lifetime()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                _validationRegistrar.AutoRegisterScopedValidators(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                _services
                    .Where(descriptor => descriptor.ServiceType == LifetimeValidationInvoker.CreateModelValidatorKey(typeof(DummyModel)))
                    .SingleOrDefault()
                    .Lifetime
                    .Should()
                    .Be(ServiceLifetime.Scoped);
            }
        }

        public class AutoRegisterSingletonValidators : LifetimeValidationRegistrarBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() =>
                {
                    _validationRegistrar.AutoRegisterSingletonValidators(null);
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

                _validationRegistrar.AutoRegisterSingletonValidators(invoker, (modelType, validatorType) =>
                {
                    wasFiltered = wasFiltered || validatorType == typeof(DummyModelValidator);
                    return false;
                });

                wasFiltered.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_All_Validators_When_Predicate_Null()
            {
                var validators = new List<Type>();

                // Cannot use a ValidationInvoker in this test since not all have a default ctor (for another test)
                var registryFake = this.CreateFake<ILifetimeValidationRegistry>();

                registryFake
                    .CallsTo(fake => fake.Register(A<Type>.Ignored, A<Type>.Ignored, A<ServiceLifetime>.Ignored))
                    .Invokes(call =>
                    {
                        var validatorType = (Type) call.Arguments[1];
                        validators.Add(validatorType);
                    });

                _validationRegistrar.AutoRegisterSingletonValidators(registryFake.FakedObject, null);

                validators.Should().HaveCount(30);      // All non-abstract validators in this assembly

                validators.All(validator => !validator.IsAbstract).Should().BeTrue();
            }

            [Fact]
            public void Should_Throw_When_Validator_Already_Registered()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                _validationRegistrar.AutoRegisterSingletonValidators(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                // registering the validator a second time will throw an error
                Invoking(() =>
                {
                    ((ILifetimeValidationRegistry) invoker).Register<DummyModel, DummyModelValidator>(Create<ServiceLifetime>());
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }

            [Fact]
            public void Should_Register_Validator_With_Lifetime()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                _validationRegistrar.AutoRegisterSingletonValidators(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                _services
                    .Where(descriptor => descriptor.ServiceType == LifetimeValidationInvoker.CreateModelValidatorKey(typeof(DummyModel)))
                    .SingleOrDefault()
                    .Lifetime
                    .Should()
                    .Be(ServiceLifetime.Singleton);
            }
        }

        public class AutoRegisterValidators : LifetimeValidationRegistrarBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() =>
                {
                    _validationRegistrar.AutoRegisterValidators(null, Create<ServiceLifetime>());
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

                _validationRegistrar.AutoRegisterValidators(invoker, Create<ServiceLifetime>(), (modelType, validatorType) =>
                {
                    wasFiltered = wasFiltered || validatorType == typeof(DummyModelValidator);
                    return false;
                });

                wasFiltered.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_All_Validators_When_Predicate_Null()
            {
                var validators = new List<Type>();

                // Cannot use a ValidationInvoker in this test since not all have a default ctor (for another test)
                var registryFake = this.CreateFake<ILifetimeValidationRegistry>();

                registryFake
                    .CallsTo(fake => fake.Register(A<Type>.Ignored, A<Type>.Ignored, A<ServiceLifetime>.Ignored))
                    .Invokes(call =>
                    {
                        var validatorType = (Type) call.Arguments[1];
                        validators.Add(validatorType);
                    });

                _validationRegistrar.AutoRegisterValidators(registryFake.FakedObject, Create<ServiceLifetime>(), null);

                validators.Should().HaveCount(30);      // All non-abstract validators in this assembly

                validators.All(validator => !validator.IsAbstract).Should().BeTrue();
            }

            [Fact]
            public void Should_Throw_When_Validator_Already_Registered()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                _validationRegistrar.AutoRegisterValidators(invoker, Create<ServiceLifetime>(), (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                // registering the validator a second time will throw an error
                Invoking(() =>
                {
                    ((ILifetimeValidationRegistry) invoker).Register<DummyModel, DummyModelValidator>(Create<ServiceLifetime>());
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }

            [Fact]
            public void Should_Register_Validator_With_Lifetime()
            {
                var lifetime = Create<ServiceLifetime>();
                var invoker = new LifetimeValidationInvoker(_services);

                _validationRegistrar.AutoRegisterValidators(invoker, lifetime, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                _services
                    .Where(descriptor => descriptor.ServiceType == LifetimeValidationInvoker.CreateModelValidatorKey(typeof(DummyModel)))
                    .SingleOrDefault()
                    .Lifetime
                    .Should()
                    .Be(lifetime);
            }
        }
    }
}
