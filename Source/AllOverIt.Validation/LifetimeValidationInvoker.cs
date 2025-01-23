using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Validation.Exceptions;
using AllOverIt.Validation.Extensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Validation
{
    // Must be internal since the caller must be able to set the provider associated with the service collection.
    internal class LifetimeValidationInvoker : ILifetimeValidationRegistry, ILifetimeValidationInvoker
    {
        [ExcludeFromCodeCoverage]
        private sealed class ModelMarker<TType, ValidatorBase>();

        private readonly IServiceCollection _services;
        private IServiceScopeFactory? _scopeFactory;

        private ILifetimeValidationRegistry ValidationRegistry => this;

        internal LifetimeValidationInvoker(IServiceCollection services)
        {
            _services = services.WhenNotNull();
        }

        bool ILifetimeValidationRegistry.ContainsModelRegistration(Type modelType)
        {
            var validatorKey = CreateModelValidatorKey(modelType);

            return _services.Any(item => item.ServiceType == validatorKey);
        }

        [SuppressMessage("Usage", "CA2263:Prefer generic overload when type is known", Justification = "Would be a recursive call, resulting in a stack overflow")]
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
            // GetModelType() asserts validatorType inherits typeof(ValidatorBase<>)
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
            var (scope, validator) = GetValidator<TType>();

            using (scope)
            {
                return validator.Validate(instance);
            }
        }

        public Task<ValidationResult> ValidateAsync<TType>(TType instance, CancellationToken cancellationToken = default)
        {
            var (scope, validator) = GetValidator<TType>();

            using (scope)
            {
                return validator.ValidateAsync(instance, cancellationToken);
            }
        }

        public ValidationResult Validate<TType, TContext>(TType instance, TContext context)
        {
            var (scope, validator) = GetValidator<TType>();

            using (scope)
            {
                return validator.Validate(instance, context);
            }
        }

        public async Task<ValidationResult> ValidateAsync<TType, TContext>(TType instance, TContext context, CancellationToken cancellationToken = default)
        {
            var (scope, validator) = GetValidator<TType>();

            using (scope)
            {
                return await validator.ValidateAsync(instance, context, cancellationToken);
            }
        }

        public void AssertValidation<TType>(TType instance)
        {
            var (scope, validator) = GetValidator<TType>();

            using (scope)
            {
                validator.ValidateAndThrow(instance);
            }
        }

        public async Task AssertValidationAsync<TType>(TType instance, CancellationToken cancellationToken = default)
        {
            var (scope, validator) = GetValidator<TType>();

            using (scope)
            {
                await validator.ValidateAndThrowAsync(instance, cancellationToken);
            }
        }

        public void AssertValidation<TType, TContext>(TType instance, TContext context)
        {
            var (scope, validator) = GetValidator<TType>();

            using (scope)
            {
                validator.ValidateAndThrow(instance, context);
            }
        }

        public async Task AssertValidationAsync<TType, TContext>(TType instance, TContext context, CancellationToken cancellationToken = default)
        {
            var (scope, validator) = GetValidator<TType>();

            using (scope)
            {
                await validator.ValidateAndThrowAsync(instance, context, cancellationToken);
            }
        }

        internal void SetScopeFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory ??= scopeFactory;
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
                provider => (IValidator)ActivatorUtilities.CreateInstance(provider, validatorType), lifetime);

            _services.Add(descriptor);
        }

        private (IServiceScope? scope, ValidatorBase<TType> validator) GetValidator<TType>()
        {
            var modelType = typeof(TType);

            if (!TryGetValidator(modelType, out var scopedValidator))
            {
                ThrowValidatorNotRegistered<TType>();
            }

            return (scopedValidator.Scope, (ValidatorBase<TType>)scopedValidator.Validator!);
        }

        private bool TryGetValidator(Type modelType, out (IServiceScope? Scope, IValidator? Validator) scopedValidator)
        {
            Throw<InvalidOperationException>.WhenNull(_scopeFactory, "The scope factory has not been set.");

            // The LifetimeValidationInvoker is a singleton so we need to create a new scope
            // so transient / scoped validators are resolved correctly (and not become singletons).
            //
            // The scope is disposed by the caller to ensure any IDisposable dependencies are not
            // disposed of until the validation has been performed.
            IValidator? validator = null;
            var scope = _scopeFactory.CreateScope();

            try
            {
                var validatorKey = CreateModelValidatorKey(modelType);

                validator = (IValidator?)scope.ServiceProvider.GetService(validatorKey);
            }
            finally
            {
                if (validator is null)
                {
                    scope.Dispose();
                    scope = null;
                }
            }

            scopedValidator.Scope = scope;
            scopedValidator.Validator = validator;

            return validator is not null;
        }

        [DoesNotReturn]
        private static void ThrowValidatorNotRegistered<TType>()
        {
            throw new InvalidOperationException($"The type '{typeof(TType).GetFriendlyName()}' does not have a registered validator.");
        }
    }
}