using AllOverIt.Extensions;
using AllOverIt.Validation.Exceptions;
using AllOverIt.Validation.Extensions;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace AllOverIt.Validation
{
    /// <summary>A validation invoker that utilizes a registry to determine which validator to invoke based on the model type.</summary>
    public sealed class ValidationInvoker : IValidationRegistry, IValidationInvoker
    {
        // can only re-use validators that don't store state (context)
        private readonly IDictionary<Type, Lazy<IValidator>> _validatorCache = new Dictionary<Type, Lazy<IValidator>>();

        /// <inheritdoc />
        public IValidationRegistry Register<TType, TValidator>()
            where TValidator : ValidatorBase<TType>, new()
        {
            _validatorCache.Add(typeof(TType), new Lazy<IValidator>(() => new TValidator()));

            return this;
        }

        /// <inheritdoc />
        /// <remarks>The validator must implement <see cref="ValidatorBase{TType}"/> where TType is the model type.</remarks>
        public IValidationRegistry Register(Type modelType, Type validatorType)
        {
            if (!validatorType.IsDerivedFrom(typeof(ValidatorBase<>)))
            {
                throw new ValidationRegistryException($"The {validatorType.GetFriendlyName()} type is not a validator.");
            }

            var validatorArgType = validatorType.BaseType!.GenericTypeArguments[0];

            if (modelType != validatorArgType)
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
        public ValidationResult Validate<TType, TContext>(TType instance, TContext context)
        {
            var validator = GetValidator<TType>();

            return validator.Validate(instance, context);
        }

        /// <inheritdoc />
        public void AssertValidation<TType>(TType instance)
        {
            var validator = GetValidator<TType>();

            // Throws a ValidationException if any rules are violated.
            validator.ValidateAndThrow(instance);
        }

        /// <inheritdoc />
        public void AssertValidation<TType, TContext>(TType instance, TContext context)
        {
            var validator = GetValidator<TType>();

            // Throws a ValidationException if any rules are violated.
            validator.ValidateAndThrow(instance, context);
        }

        private ValidatorBase<TType> GetValidator<TType>()
        {
            if (!_validatorCache.TryGetValue(typeof(TType), out var resolver))
            {
                ThrowValidatorNotRegistered<TType>();
            }

            return (ValidatorBase<TType>)resolver!.Value;
        }

        private static void ThrowValidatorNotRegistered<TType>()
        {
            throw new InvalidOperationException($"The type '{typeof(TType).GetFriendlyName()}' does not have a registered validator.");
        }
    }
}