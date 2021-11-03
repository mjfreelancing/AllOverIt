using FluentValidation.Results;

namespace AllOverIt.Validation
{
    /// <summary>Represents a validation invoker.</summary>
    public interface IValidationInvoker
    {
        /// <summary>Validates a model instance.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <param name="instance">The model instance.</param>
        /// <returns>The validation result.</returns>
        ValidationResult Validate<TType>(TType instance);

        /// <summary>Validates a model instance. Additional context data is associated with the request that can be utilized in the validation rules.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="context">The context type.</param>
        /// <returns>The validation result.</returns>
        ValidationResult Validate<TType, TContext>(TType instance, TContext context);

        /// <summary>Validates a model instance. If the validation fails then a validation exception is thrown.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <param name="instance">The model instance.</param>
        void AssertValidation<TType>(TType instance);

        /// <summary>Validates a model instance. If the validation fails then a validation exception is thrown.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="context">The context type.</param>
        void AssertValidation<TType, TContext>(TType instance, TContext context);
    }
}