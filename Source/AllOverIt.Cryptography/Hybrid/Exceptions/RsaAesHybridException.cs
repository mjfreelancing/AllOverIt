using System;

namespace AllOverIt.Cryptography.Hybrid.Exceptions
{
    /// <summary>Exception raised during an RSA-AES hybrid encryption or decryption operation.</summary>
    public class RsaAesHybridException : Exception
    {
        /// <summary>Default constructor.</summary>
        public RsaAesHybridException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public RsaAesHybridException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RsaAesHybridException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
