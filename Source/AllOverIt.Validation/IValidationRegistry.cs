using System;

namespace AllOverIt.Validation
{
    /// <summary>Represents a registry of model types and their associated validators.</summary>
    public interface IValidationRegistry
    {
        /// <summary>Registers a model type with an associated validator.</summary>
        /// <typeparam name="TType">The model type.</typeparam>
        /// <typeparam name="TValidator">The validator type.</typeparam>
        /// <returns>The registry, allowing for a fluent syntax.</returns>
        IValidationRegistry Register<TType, TValidator>() where TValidator : ValidatorBase<TType>, new();

        /// <summary>Registers a model type with an associated validator type.</summary>
        /// <param name="modelType">The model type.</param>
        /// <param name="validatorType">The validator type.</param>
        /// <returns>The registry, allowing for a fluent syntax.</returns>
        IValidationRegistry Register(Type modelType, Type validatorType);
    }
}