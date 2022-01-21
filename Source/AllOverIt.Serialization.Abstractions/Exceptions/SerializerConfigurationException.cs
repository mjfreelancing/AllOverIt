using System;

namespace AllOverIt.Serialization.Abstractions.Exceptions
{
    /// <summary>Thrown when unable to set a configuration option on an <see cref="IJsonSerializer"/>.</summary>
    public class SerializerConfigurationException : Exception
    {
        /// <summary>Default constructor.</summary>
        public SerializerConfigurationException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public SerializerConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SerializerConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
