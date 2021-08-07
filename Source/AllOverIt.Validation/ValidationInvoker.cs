using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using ValidationException = AllOverIt.Validation.Exceptions.ValidationException;

namespace AllOverIt.Validation
{
    public sealed class ValidationInvoker : IValidationRegistry, IValidationInvoker
    {
        // can only re-use validators that don't store state (context)
        private readonly IDictionary<Type, Lazy<object>> _validatorCache = new Dictionary<Type, Lazy<object>>();

        public IValidationRegistry Register<TType, TValidator>()
            where TType : class
            where TValidator : ValidatorBase<TType>, new()
        {
            _validatorCache.Add(typeof(TType), new Lazy<object>(() => new TValidator()));

            return this;
        }

        public void AssertValidation<TType>(TType instance)
            where TType : class
        {
            _ = instance.WhenNotNull(nameof(instance));

            var validator = GetValidator<TType>();

            var validationResult = validator.Validate(instance);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }

        public void AssertValidation<TType, TContext>(TType instance, TContext context)
            where TType : class
        {
            _ = instance.WhenNotNull(nameof(instance));

            var validator = GetValidator<TType>();

            var validationResult = validator.Validate(instance, context);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }

        private ValidatorBase<TType> GetValidator<TType>()
        {
            if (!_validatorCache.TryGetValue(typeof(TType), out var resolver))
            {
                ThrowValidatorNotRegistered<TType>();
            }

            return (ValidatorBase<TType>)resolver.Value;
        }

        private static void ThrowValidatorNotRegistered<TType>()
        {
            throw new InvalidOperationException($"The type '{typeof(TType).GetFriendlyName()}' does not have a registered validator.");
        }
    }
}