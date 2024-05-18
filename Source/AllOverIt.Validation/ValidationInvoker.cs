using AllOverIt.Extensions;
using AllOverIt.Validation.Exceptions;
using AllOverIt.Validation.Extensions;
using FluentValidation;
using FluentValidation.Results;

namespace AllOverIt.Validation
{
    /// <summary>A validation invoker that utilizes a registry to determine which validator to invoke based on the model type.</summary>
    public class ValidationInvoker : IValidationRegistry, IValidationInvoker
    {
        // can only re-use validators that don't store state (context)
        private readonly Dictionary<Type, Lazy<IValidator>> _validatorCache = [];

        private IValidationRegistry ValidationRegistry => this;

        /// <summary>Constructor.</summary>
        public ValidationInvoker()
        {
        }

        // For tests
        internal ValidationInvoker(Dictionary<Type, Lazy<IValidator>> validatorCache)
        {
            _validatorCache = validatorCache;
        }

        /// <inheritdoc />
        bool IValidationRegistry.ContainsModelRegistration(Type modelType)
        {
            return _validatorCache.ContainsKey(modelType);
        }

        /// <inheritdoc />
#pragma warning disable IDE0079 // Remove unnecessary suppression
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2263:Prefer generic overload when type is known", Justification = "Would be a recursive call, resulting in a stack overflow")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
        bool IValidationRegistry.ContainsModelRegistration<TType>()
        {
            return ValidationRegistry.ContainsModelRegistration(typeof(TType));
        }

        /// <inheritdoc />
        IValidationRegistry IValidationRegistry.Register<TType, TValidator>()
        {
            AddToValidatorCache(typeof(TType), new Lazy<IValidator>(() => new TValidator()));

            return this;
        }

        /// <inheritdoc />
        /// <remarks>The validator must implement <see cref="ValidatorBase{TType}"/> where TType is the model type.</remarks>
        IValidationRegistry IValidationRegistry.Register(Type modelType, Type validatorType)
        {
            if (!validatorType.IsDerivedFrom(typeof(ValidatorBase<>)))
            {
                throw new ValidationRegistryException($"The type '{validatorType.GetFriendlyName()}' is not a validator.");
            }

            if (validatorType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new ValidationRegistryException($"The type '{validatorType.GetFriendlyName()}' must have a default constructor.");
            }

            var validatorModelType = ValidationTypeHelper.GetModelType(validatorType);

            if (modelType != validatorModelType)
            {
                throw new ValidationRegistryException($"The type '{validatorType.GetFriendlyName()}' cannot validate a {modelType.GetFriendlyName()} type.");
            }

            AddToValidatorCache(modelType, new Lazy<IValidator>(() => (IValidator) Activator.CreateInstance(validatorType)));

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

        private void AddToValidatorCache(Type modelType, Lazy<IValidator> factory)
        {
            if (ValidationRegistry.ContainsModelRegistration(modelType))
            {
                throw new ValidationRegistryException($"The type '{modelType.GetFriendlyName()}' already has a registered validator.");
            }

            _validatorCache.Add(modelType, factory);
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