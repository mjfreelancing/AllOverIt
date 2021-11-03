using AllOverIt.Extensions;
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
        private readonly IDictionary<Type, Lazy<object>> _validatorCache = new Dictionary<Type, Lazy<object>>();

        /// <inheritdoc />
        public IValidationRegistry Register<TType, TValidator>() where TValidator : ValidatorBase<TType>, new()
        {
            _validatorCache.Add(typeof(TType), new Lazy<object>(() => new TValidator()));

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