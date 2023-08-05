using System;

namespace AllOverIt.Cryptography.AES.Exceptions
{
    public class AesException : Exception
    {
        public AesException()
        {
        }

        public AesException(string message)
            : base(message)
        {
        }

        public AesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
