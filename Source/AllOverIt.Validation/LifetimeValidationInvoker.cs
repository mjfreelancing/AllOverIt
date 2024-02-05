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
    /// <summary>A validation invoker that utilizes a registry to determine which validator to invoke based on the model type.</summary>
    public class LifetimeValidationInvoker : ILifetimeValidationRegistry, ILifetimeValidationInvoker
    {
        private sealed class ModelMarker<TType, ValidatorBase>()
        {
        }

        private readonly IServiceCollection _services;
        private IServiceProvider _serviceProvider;

        internal LifetimeValidationInvoker(IServiceCollection services)
        {
            _services = services.WhenNotNull(nameof(services));
        }

        /// <inheritdoc />
        public bool ContainsModelRegistration(Type modelType)
        {
            var validatorKey = CreateModelValidatorKey(modelType);

            return _services.Any(item => item.ServiceType == validatorKey);
        }

        /// <inheritdoc />
        public bool ContainsModelRegistration<TType>()
        {
            return ContainsModelRegistration(typeof(TType));
        }

        /// <inheritdoc />
        public ILifetimeValidationRegistry RegisterTransient<TType, TValidator>()
            where TValidator : ValidatorBase<TType>, new()
        {
            return Register(typeof(TType), typeof(TValidator), ServiceLifetime.Transient);
        }

        /// <inheritdoc />
        public ILifetimeValidationRegistry RegisterScoped<TType, TValidator>()
            where TValidator : ValidatorBase<TType>, new()
        {
            return Register(typeof(TType), typeof(TValidator), ServiceLifetime.Scoped);
        }

        /// <inheritdoc />
        public ILifetimeValidationRegistry RegisterSingleton<TType, TValidator>()
            where TValidator : ValidatorBase<TType>, new()
        {
            return Register(typeof(TType), typeof(TValidator), ServiceLifetime.Singleton);
        }

        /// <inheritdoc />
        public ILifetimeValidationRegistry Register<TType, TValidator>(ServiceLifetime lifetime)
            where TValidator : ValidatorBase<TType>, new()
        {
            return Register(typeof(TType), typeof(TValidator), lifetime);
        }

        /// <inheritdoc />
        public ILifetimeValidationRegistry RegisterTransient(Type modelType, Type validatorType)
        {
            return Register(modelType, validatorType, ServiceLifetime.Transient);
        }

        /// <inheritdoc />
        public ILifetimeValidationRegistry RegisterScoped(Type modelType, Type validatorType)
        {
            return Register(modelType, validatorType, ServiceLifetime.Scoped);
        }

        /// <inheritdoc />
        public ILifetimeValidationRegistry RegisterSingleton(Type modelType, Type validatorType)
        {
            return Register(modelType, validatorType, ServiceLifetime.Singleton);
        }

        /// <inheritdoc />
        public ILifetimeValidationRegistry Register(Type modelType, Type validatorType, ServiceLifetime lifetime)
        {
            if (!validatorType.IsDerivedFrom(typeof(ValidatorBase<>)))
            {
                throw new ValidationRegistryException($"The {validatorType.GetFriendlyName()} type is not a validator.");
            }

            var validatorModelType = ValidationTypeHelper.GetModelType(validatorType);

            if (modelType != validatorModelType)
            {
                throw new ValidationRegistryException($"The {validatorType.GetFriendlyName()} type cannot validate a {modelType} type.");
            }

            RegisterModelValidator(modelType, validatorType, lifetime);

            return this;
        }

        /// <inheritdoc />
        public ValidationResult Validate<TType>(TType instance)
        {
            var validator = GetValidator<TType>();

            return validator.Validate(instance);
        }

        /// <inheritdoc />
        public Task<ValidationResult> ValidateAsync<TType>(TType instance, CancellationToken cancellationToken = default)
        {
            var validator = GetValidator<TType>();

            return validator.ValidateAsync(instance, cancellationToken);
        }

        /// <inheritdoc />
        public ValidationResult Validate<TType, TContext>(TType instance, TContext context)
        {
            var validator = GetValidator<TType>();

            return validator.Validate(instance, context);
        }

        /// <inheritdoc />
        public Task<ValidationResult> ValidateAsync<TType, TContext>(TType instance, TContext context, CancellationToken cancellationToken = default)
        {
            var validator = GetValidator<TType>();

            return validator.ValidateAsync(instance, context, cancellationToken);
        }

        /// <inheritdoc />
        public void AssertValidation<TType>(TType instance)
        {
            var validator = GetValidator<TType>();

            validator.ValidateAndThrow(instance);
        }

        /// <inheritdoc />
        public Task AssertValidationAsync<TType>(TType instance, CancellationToken cancellationToken = default)
        {
            var validator = GetValidator<TType>();

            return validator.ValidateAndThrowAsync(instance, cancellationToken);
        }

        /// <inheritdoc />
        public void AssertValidation<TType, TContext>(TType instance, TContext context)
        {
            var validator = GetValidator<TType>();

            validator.ValidateAndThrow(instance, context);
        }

        /// <inheritdoc />
        public Task AssertValidationAsync<TType, TContext>(TType instance, TContext context, CancellationToken cancellationToken = default)
        {
            var validator = GetValidator<TType>();

            return validator.ValidateAndThrowAsync(instance, context, cancellationToken);
        }

        internal void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider ??= serviceProvider;
        }

        private void RegisterModelValidator(Type modelType, Type validatorType, ServiceLifetime lifetime)
        {
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
            var validatorKey = CreateModelValidatorKey(modelType);

            validator = (IValidator) _serviceProvider.GetService(validatorKey);

            return validator is not null;
        }

        private Type CreateModelValidatorKey(Type modelType)
        {
            var modelValidatorType = typeof(ValidatorBase<>).MakeGenericType(modelType);

            return typeof(ModelMarker<,>).MakeGenericType(modelType, modelValidatorType);
        }

        private static void ThrowValidatorNotRegistered<TType>()
        {
            throw new InvalidOperationException($"The type '{typeof(TType).GetFriendlyName()}' does not have a registered validator.");
        }
    }
}