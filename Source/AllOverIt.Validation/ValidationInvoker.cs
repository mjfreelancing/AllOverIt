using AllOverIt.Extensions;
using AllOverIt.Validation.Exceptions;
using AllOverIt.Validation.Extensions;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Validation
{
    /// <summary>A validation invoker that utilizes a registry to determine which validator to invoke based on the model type.</summary>
    public class ValidationInvoker : IValidationRegistry, IValidationInvoker
    {
        // can only re-use validators that don't store state (context)
        private readonly Dictionary<Type, Lazy<IValidator>> _validatorCache = [];

        /// <inheritdoc />
        public IValidationRegistry Register<TType, TValidator>()
            where TValidator : ValidatorBase<TType>, new()
        {
            _validatorCache.Add(typeof(TType), new Lazy<IValidator>(() => new TValidator()));

            return this;
        }

        /// <inheritdoc />
        public bool ContainsModelRegistration(Type modelType)
        {
            return _validatorCache.ContainsKey(modelType);
        }

        /// <inheritdoc />
        public bool ContainsModelRegistration<TType>()
        {
            return ContainsModelRegistration(typeof(TType));
        }

        /// <inheritdoc />
        /// <remarks>The validator must implement <see cref="ValidatorBase{TType}"/> where TType is the model type.</remarks>
        public IValidationRegistry Register(Type modelType, Type validatorType)
        {
            if (!validatorType.IsDerivedFrom(typeof(ValidatorBase<>)))
            {
                throw new ValidationRegistryException($"The {validatorType.GetFriendlyName()} type is not a validator.");
            }

            if (validatorType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new ValidationRegistryException($"The {validatorType.GetFriendlyName()} type must have a default constructor.");
            }

            var validatorModelType = ValidationTypeHelper.GetModelType(validatorType);

            if (modelType != validatorModelType)
            {
                throw new ValidationRegistryException($"The {validatorType.GetFriendlyName()} type cannot validate a {modelType} type.");
            }

            _validatorCache.Add(modelType, new Lazy<IValidator>(() => (IValidator) Activator.CreateInstance(validatorType)));

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

        private ValidatorBase<TType> GetValidator<TType>()
        {
            var modelType = typeof(TType);

            if (!_validatorCache.TryGetValue(modelType, out var resolver))
            {
                ThrowValidatorNotRegistered<TType>();
            }

            return (ValidatorBase<TType>) resolver!.Value;
        }

        private static void ThrowValidatorNotRegistered<TType>()
        {
            throw new InvalidOperationException($"The type '{typeof(TType).GetFriendlyName()}' does not have a registered validator.");
        }
    }
}