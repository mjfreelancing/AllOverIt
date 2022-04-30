using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
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

        /// <summary>Asynchronously validates a model instance.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>The validation result.</returns>
        Task<ValidationResult> ValidateAsync<TType>(TType instance, CancellationToken cancellationToken = default);

        /// <summary>Validates a model instance. Additional context data is associated with the request that can be utilized
        /// in the validation rules.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="context">The context type.</param>
        /// <returns>The validation result.</returns>
        ValidationResult Validate<TType, TContext>(TType instance, TContext context);

        /// <summary>Asynchronously validates a model instance. Additional context data is associated with the request that can be utilized
        /// in the validation rules.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="context">The context type.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>The validation result.</returns>
        Task<ValidationResult> ValidateAsync<TType, TContext>(TType instance, TContext context, CancellationToken cancellationToken = default);

        /// <summary>Validates a model instance. If the validation fails then a validation exception is thrown.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <param name="instance">The model instance.</param>
        /// <exception cref="ValidationException">When any rules are violated.</exception>
        void AssertValidation<TType>(TType instance);

        /// <summary>Asynchronously validates a model instance. If the validation fails then a validation exception is thrown.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <exception cref="ValidationException">When any rules are violated.</exception>
        Task AssertValidationAsync<TType>(TType instance, CancellationToken cancellationToken = default);

        /// <summary>Validates a model instance. If the validation fails then a validation exception is thrown.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="context">The context type.</param>
        /// <exception cref="ValidationException">When any rules are violated.</exception>
        void AssertValidation<TType, TContext>(TType instance, TContext context);

        /// <summary>Asynchronously validates a model instance. If the validation fails then a validation exception is thrown.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="context">The context type.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <exception cref="ValidationException">When any rules are violated.</exception>
        Task AssertValidationAsync<TType, TContext>(TType instance, TContext context, CancellationToken cancellationToken = default);
    }
}