using System;

namespace AllOverIt.Cryptography.RSA.Exceptions
{
    /// <summary>Exception raised during an RSA encryption or decryption operation.</summary>
    public class RsaException : Exception
    {
        /// <summary>Default constructor.</summary>
        public RsaException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public RsaException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RsaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
