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
        /// <param name="validatorArgsResolver">When used, this provides the arguments to be passed to the validator constructor
        /// when it is instantiated. When not used it is assumed the default constructor will be used.</param>
        /// <returns>The registry, allowing for a fluent syntax.</returns>
        IValidationRegistry Register(Type modelType, Type validatorType, Func<object[]> validatorArgsResolver = default);
    }
}