using AllOverIt.Extensions;
using AllOverIt.Validation.Extensions;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace AllOverIt.Validation
{
    public sealed class ValidationInvoker : IValidationRegistry, IValidationInvoker
    {
        // can only re-use validators that don't store state (context)
        private readonly IDictionary<Type, Lazy<object>> _validatorCache = new Dictionary<Type, Lazy<object>>();

        public IValidationRegistry Register<TType, TValidator>() where TValidator : ValidatorBase<TType>, new()
        {
            _validatorCache.Add(typeof(TType), new Lazy<object>(() => new TValidator()));

            return this;
        }

        public ValidationResult Validate<TType>(TType instance)
        {
            var validator = GetValidator<TType>();

            return validator.Validate(instance);
        }

        public ValidationResult Validate<TType, TContext>(TType instance, TContext context)
        {
            var validator = GetValidator<TType>();

            return validator.Validate(instance, context);
        }

        // Throws a ValidationException if any rules are violated.
        public void AssertValidation<TType>(TType instance)
        {
            var validator = GetValidator<TType>();

            validator.ValidateAndThrow(instance);
        }

        // Throws a ValidationException if any rules are violated.
        public void AssertValidation<TType, TContext>(TType instance, TContext context)
        {
            var validator = GetValidator<TType>();

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