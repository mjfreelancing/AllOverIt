using System;

namespace AllOverIt.Patterns.ValueObject.Exceptions
{
    /// <summary>Represents a validation error that occurred while initializing a <see cref="ValueObject{TValue, TType}"/>.</summary>
    public sealed class ValueObjectValidationException : Exception
    {
        /// <summary>The value that failed validation.</summary>
        public object? AttemptedValue { get; }

        /// <summary>Constructor.</summary>
        /// <param name="attemptedValue">The value that failed validation.</param>
        /// <param name="message">The exception message.</param>
        internal ValueObjectValidationException(object? attemptedValue, string message)
            : base(message)
        {
            AttemptedValue = attemptedValue;
        }
    }
}