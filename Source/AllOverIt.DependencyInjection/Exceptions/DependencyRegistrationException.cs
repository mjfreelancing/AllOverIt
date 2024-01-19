using System;

namespace AllOverIt.DependencyInjection.Exceptions
{
    /// <summary>Represents an error that occurred while auto-registering a service for dependency injection.</summary>
    public sealed class DependencyRegistrationException : Exception
    {
        /// <summary>Default constructor.</summary>
        public DependencyRegistrationException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public DependencyRegistrationException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DependencyRegistrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}