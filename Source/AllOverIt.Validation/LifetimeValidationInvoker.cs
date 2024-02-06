using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Validation.Exceptions;
using AllOverIt.Validation.Extensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Validation
{
    // Must be internal since the caller must be able to set the provider associated with the service collection.
    internal class LifetimeValidationInvoker : ILifetimeValidationRegistry, ILifetimeValidationInvoker
    {
        private sealed class ModelMarker<TType, ValidatorBase>()
        {
        }

        private readonly IServiceCollection _services;
        private IServiceProvider _serviceProvider;

        private ILifetimeValidationRegistry ValidationRegistry => this;

        internal LifetimeValidationInvoker(IServiceCollection services)
        {
            _services = services.WhenNotNull(nameof(services));
        }

        bool ILifetimeValidationRegistry.ContainsModelRegistration(Type modelType)
        {
            var validatorKey = CreateModelValidatorKey(modelType);

            return _services.Any(item => item.ServiceType == validatorKey);
        }

        bool ILifetimeValidationRegistry.ContainsModelRegistration<TType>()
        {
            return ValidationRegistry.ContainsModelRegistration(typeof(TType));
        }

        ILifetimeValidationRegistry ILifetimeValidationRegistry.RegisterTransient<TType, TValidator>()
        {
            // Don't need to validate the model / validator combination
            RegisterModelValidator(typeof(TType), typeof(TValidator), ServiceLifetime.Transient);

            return this;
        }

        ILifetimeValidationRegistry ILifetimeValidationRegistry.RegisterScoped<TType, TValidator>()
        {
            // Don't need to validate the model / validator combination
            RegisterModelValidator(typeof(TType), typeof(TValidator), ServiceLifetime.Scoped);

            return this;
        }

        ILifetimeValidationRegistry ILifetimeValidationRegistry.RegisterSingleton<TType, TValidator>()
        {
            // Don't need to validate the model / validator combination
            RegisterModelValidator(typeof(TType), typeof(TValidator), ServiceLifetime.Singleton);

            return this;
        }

        ILifetimeValidationRegistry ILifetimeValidationRegistry.Register<TType, TValidator>(ServiceLifetime lifetime)
        {
            // Don't need to validate the model / validator combination
            RegisterModelValidator(typeof(TType), typeof(TValidator), lifetime);

            return this;
        }

        ILifetimeValidationRegistry ILifetimeValidationRegistry.RegisterTransient(Type modelType, Type validatorType)
        {
            return ValidationRegistry.Register(modelType, validatorType, ServiceLifetime.Transient);
        }

        ILifetimeValidationRegistry ILifetimeValidationRegistry.RegisterScoped(Type modelType, Type validatorType)
        {
            return ValidationRegistry.Register(modelType, validatorType, ServiceLifetime.Scoped);
        }

        ILifetimeValidationRegistry ILifetimeValidationRegistry.RegisterSingleton(Type modelType, Type validatorType)
        {
            return ValidationRegistry.Register(modelType, validatorType, ServiceLifetime.Singleton);
        }

        ILifetimeValidationRegistry ILifetimeValidationRegistry.Register(Type modelType, Type validatorType, ServiceLifetime lifetime)
        {
            if (!validatorType.IsDerivedFrom(typeof(ValidatorBase<>)))
            {
                throw new ValidationRegistryException($"The type '{validatorType.GetFriendlyName()}' is not a validator.");
            }

            var validatorModelType = ValidationTypeHelper.GetModelType(validatorType);

            if (modelType != validatorModelType)
            {
                throw new ValidationRegistryException($"The type '{validatorType.GetFriendlyName()}' cannot validate a {modelType.GetFriendlyName()} type.");
            }

            RegisterModelValidator(modelType, validatorType, lifetime);

            return this;
        }

        public ValidationResult Validate<TType>(TType instance)
        {
            var validator = GetValidator<TType>();

            return validator.Validate(instance);
        }

        public Task<ValidationResult> ValidateAsync<TType>(TType instance, CancellationToken cancellationToken = default)
        {
            var validator = GetValidator<TType>();

            return validator.ValidateAsync(instance, cancellationToken);
        }

        public ValidationResult Validate<TType, TContext>(TType instance, TContext context)
        {
            var validator = GetValidator<TType>();

            return validator.Validate(instance, context);
        }

        public Task<ValidationResult> ValidateAsync<TType, TContext>(TType instance, TContext context, CancellationToken cancellationToken = default)
        {
            var validator = GetValidator<TType>();

            return validator.ValidateAsync(instance, context, cancellationToken);
        }

        public void AssertValidation<TType>(TType instance)
        {
            var validator = GetValidator<TType>();

            validator.ValidateAndThrow(instance);
        }

        public Task AssertValidationAsync<TType>(TType instance, CancellationToken cancellationToken = default)
        {
            var validator = GetValidator<TType>();

            return validator.ValidateAndThrowAsync(instance, cancellationToken);
        }

        public void AssertValidation<TType, TContext>(TType instance, TContext context)
        {
            var validator = GetValidator<TType>();

            validator.ValidateAndThrow(instance, context);
        }

        public Task AssertValidationAsync<TType, TContext>(TType instance, TContext context, CancellationToken cancellationToken = default)
        {
            var validator = GetValidator<TType>();

            return validator.ValidateAndThrowAsync(instance, context, cancellationToken);
        }

        internal void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider ??= serviceProvider;
        }

        internal static Type CreateModelValidatorKey(Type modelType)
        {
            var modelValidatorType = typeof(ValidatorBase<>).MakeGenericType(modelType);

            return typeof(ModelMarker<,>).MakeGenericType(modelType, modelValidatorType);
        }

        private void RegisterModelValidator(Type modelType, Type validatorType, ServiceLifetime lifetime)
        {
            if (ValidationRegistry.ContainsModelRegistration(modelType))
            {
                throw new ValidationRegistryException($"The type '{modelType.GetFriendlyName()}' already has a registered validator.");
            }

            var validatorKey = CreateModelValidatorKey(modelType);

            var descriptor = new ServiceDescriptor(
                validatorKey,
                provider => (IValidator) ActivatorUtilities.CreateInstance(provider, validatorType), lifetime);

            _services.Add(descriptor);
        }

        private ValidatorBase<TType> GetValidator<TType>()
        {
            var modelType = typeof(TType);

            if (!TryGetValidator(modelType, out var validator))
            {
                ThrowValidatorNotRegistered<TType>();
            }

            return (ValidatorBase<TType>) validator;
        }

        private bool TryGetValidator(Type modelType, out IValidator validator)
        {
            Throw<InvalidOperationException>.WhenNull(_serviceProvider, "The service provider has not been set.");

            var validatorKey = CreateModelValidatorKey(modelType);

            validator = (IValidator) _serviceProvider.GetService(validatorKey);

            return validator is not null;
        }

        private static void ThrowValidatorNotRegistered<TType>()
        {
            throw new InvalidOperationException($"The type '{typeof(TType).GetFriendlyName()}' does not have a registered validator.");
        }
    }
}