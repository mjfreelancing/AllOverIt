namespace AllOverIt.Cryptography.AES.Exceptions
{
    /// <summary>Exception raised during an AES encryption or decryption operation.</summary>
    public class AesException : Exception
    {
        /// <summary>Default constructor.</summary>
        public AesException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public AesException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
