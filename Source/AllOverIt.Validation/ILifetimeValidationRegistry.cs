using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Validation
{
    /// <summary>Represents a registry of model types and their associated validators, with a given lifetime.</summary>
    public interface ILifetimeValidationRegistry
    {
        /// <summary>Indicates if the specified model type has a registered validator.</summary>
        /// <param name="modelType">The model type.</param>
        /// <returns><see langword="True" /> if the specified type has a registered validator, otherwise <see langword="False" />.</returns>
        bool ContainsModelRegistration(Type modelType);

        /// <summary>Indicates if the specified model type has a registered validator.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <returns><see langword="True" /> if the specified type has a registered validator, otherwise <see langword="False" />.</returns>
        bool ContainsModelRegistration<TType>();

        /// <summary>Registers a model type with an associated validator, with a transient lifetime.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <typeparam name="TValidator">The validator type.</typeparam>
        /// <returns>The registry, allowing for a fluent syntax.</returns>
        ILifetimeValidationRegistry RegisterTransient<TType, TValidator>() where TValidator : ValidatorBase<TType>, new();

        /// <summary>Registers a model type with an associated validator, with a scoped lifetime.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <typeparam name="TValidator">The validator type.</typeparam>
        /// <returns>The registry, allowing for a fluent syntax.</returns>
        ILifetimeValidationRegistry RegisterScoped<TType, TValidator>() where TValidator : ValidatorBase<TType>, new();

        /// <summary>Registers a model type with an associated validator, with a singleton lifetime.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <typeparam name="TValidator">The validator type.</typeparam>
        /// <returns>The registry, allowing for a fluent syntax.</returns>
        ILifetimeValidationRegistry RegisterSingleton<TType, TValidator>() where TValidator : ValidatorBase<TType>, new();

        /// <summary>Registers a model type with an associated validator, with a singleton lifetime.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <typeparam name="TValidator">The validator type.</typeparam>
        /// <param name="lifetime">The lifetime of the validator.</param>
        /// <returns>The registry, allowing for a fluent syntax.</returns>
        ILifetimeValidationRegistry Register<TType, TValidator>(ServiceLifetime lifetime) where TValidator : ValidatorBase<TType>, new();

        /// <summary>Registers a model type with an associated validator type, with a transient lifetime.</summary>
        /// <param name="modelType">The model type.</param>
        /// <param name="validatorType">The validator type.</param>
        /// <returns>The registry, allowing for a fluent syntax.</returns>
        ILifetimeValidationRegistry RegisterTransient(Type modelType, Type validatorType);

        /// <summary>Registers a model type with an associated validator type, with a scoped lifetime.</summary>
        /// <param name="modelType">The model type.</param>
        /// <param name="validatorType">The validator type.</param>
        /// <returns>The registry, allowing for a fluent syntax.</returns>
        ILifetimeValidationRegistry RegisterScoped(Type modelType, Type validatorType);

        /// <summary>Registers a model type with an associated validator type, with a singleton lifetime.</summary>
        /// <param name="modelType">The model type.</param>
        /// <param name="validatorType">The validator type.</param>
        /// <returns>The registry, allowing for a fluent syntax.</returns>
        ILifetimeValidationRegistry RegisterSingleton(Type modelType, Type validatorType);

        /// <summary>Registers a model type with an associated validator type, with a singleton lifetime.</summary>
        /// <param name="modelType">The model type.</param>
        /// <param name="validatorType">The validator type.</param>
        /// <param name="lifetime">The lifetime of the validator.</param>
        /// <returns>The registry, allowing for a fluent syntax.</returns>
        ILifetimeValidationRegistry Register(Type modelType, Type validatorType, ServiceLifetime lifetime);
    }
}