using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Validation.Exceptions;
using AllOverIt.Validation.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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
            public void Should_Filter_Validator()
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
            public void Should_Register_Validator()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                LifetimeValidationRegistryExtensions.AutoRegisterTransientValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                AssertValidatorRegistration<DummyModel, DummyModelValidator>(_services, ServiceLifetime.Transient);
            }

            [Fact]
            public void Should_Throw_When_Already_Registered()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                LifetimeValidationRegistryExtensions.AutoRegisterTransientValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                // registering the validator a second time will throw an error
                Invoking(() =>
                {
                    ((ILifetimeValidationRegistry) invoker).RegisterTransient<DummyModel, DummyModelValidator>();
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }
        }

        public class AutoRegisterScopedValidators : LifetimeValidationRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ValidationRegistry_Null()
            {
                Invoking(() =>
                {
                    LifetimeValidationRegistryExtensions.AutoRegisterScopedValidators<DummyRegistrar>(null);
                })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("validationRegistry");
            }

            [Fact]
            public void Should_Filter_Validator()
            {
                var wasFiltered = false;

                var invoker = new LifetimeValidationInvoker(_services);

                LifetimeValidationRegistryExtensions.AutoRegisterScopedValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    wasFiltered = wasFiltered || validatorType == typeof(DummyModelValidator);
                    return false;
                });

                wasFiltered.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_Validator()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                LifetimeValidationRegistryExtensions.AutoRegisterScopedValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                AssertValidatorRegistration<DummyModel, DummyModelValidator>(_services, ServiceLifetime.Scoped);
            }

            [Fact]
            public void Should_Throw_When_Already_Registered()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                LifetimeValidationRegistryExtensions.AutoRegisterScopedValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                // registering the validator a second time will throw an error
                Invoking(() =>
                {
                    ((ILifetimeValidationRegistry) invoker).RegisterScoped<DummyModel, DummyModelValidator>();
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }
        }

        public class AutoRegisterSingletonValidators : LifetimeValidationRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ValidationRegistry_Null()
            {
                Invoking(() =>
                {
                    LifetimeValidationRegistryExtensions.AutoRegisterSingletonValidators<DummyRegistrar>(null);
                })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("validationRegistry");
            }

            [Fact]
            public void Should_Filter_Validator()
            {
                var wasFiltered = false;

                var invoker = new LifetimeValidationInvoker(_services);

                LifetimeValidationRegistryExtensions.AutoRegisterSingletonValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    wasFiltered = wasFiltered || validatorType == typeof(DummyModelValidator);
                    return false;
                });

                wasFiltered.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_Validator()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                LifetimeValidationRegistryExtensions.AutoRegisterSingletonValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                AssertValidatorRegistration<DummyModel, DummyModelValidator>(_services, ServiceLifetime.Singleton);
            }

            [Fact]
            public void Should_Throw_When_Already_Registered()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                LifetimeValidationRegistryExtensions.AutoRegisterSingletonValidators<DummyRegistrar>(invoker, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                // registering the validator a second time will throw an error
                Invoking(() =>
                {
                    ((ILifetimeValidationRegistry) invoker).RegisterScoped<DummyModel, DummyModelValidator>();
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }
        }

        public class AutoRegisterValidators : LifetimeValidationRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ValidationRegistry_Null()
            {
                Invoking(() =>
                {
                    LifetimeValidationRegistryExtensions.AutoRegisterValidators<DummyRegistrar>(null, Create<ServiceLifetime>());
                })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("validationRegistry");
            }

            [Fact]
            public void Should_Filter_Validator()
            {
                var wasFiltered = false;

                var invoker = new LifetimeValidationInvoker(_services);

                LifetimeValidationRegistryExtensions.AutoRegisterValidators<DummyRegistrar>(invoker, Create<ServiceLifetime>(), (modelType, validatorType) =>
                {
                    wasFiltered = wasFiltered || validatorType == typeof(DummyModelValidator);
                    return false;
                });

                wasFiltered.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_Validator()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                var lifetime = Create<ServiceLifetime>();

                LifetimeValidationRegistryExtensions.AutoRegisterValidators<DummyRegistrar>(invoker, lifetime, (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                AssertValidatorRegistration<DummyModel, DummyModelValidator>(_services, lifetime);
            }

            [Fact]
            public void Should_Throw_When_Already_Registered()
            {
                var invoker = new LifetimeValidationInvoker(_services);

                LifetimeValidationRegistryExtensions.AutoRegisterValidators<DummyRegistrar>(invoker, Create<ServiceLifetime>(), (modelType, validatorType) =>
                {
                    return validatorType == typeof(DummyModelValidator);
                });

                // registering the validator a second time will throw an error
                Invoking(() =>
                {
                    ((ILifetimeValidationRegistry) invoker).RegisterScoped<DummyModel, DummyModelValidator>();
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }
        }

        private static void AssertValidatorRegistration<TModel, TValidator>(IServiceCollection services, ServiceLifetime lifetime)
        {
            var descriptor = services
                .Where(item => item.ServiceType == LifetimeValidationInvoker.CreateModelValidatorKey(typeof(TModel)))
                .SingleOrDefault();

            descriptor.Lifetime.Should().Be(lifetime);

            var provider = services.BuildServiceProvider();

            descriptor.ImplementationFactory
                .Invoke(provider)
                .Should()
                .BeOfType(typeof(TValidator));
        }
    }
}
