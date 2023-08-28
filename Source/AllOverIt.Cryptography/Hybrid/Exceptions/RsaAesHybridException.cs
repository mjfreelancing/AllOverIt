using System;

namespace AllOverIt.Cryptography.Hybrid.Exceptions
{
    public class RsaAesHybridException : Exception
    {
        public RsaAesHybridException()
        {
        }

        public RsaAesHybridException(string message)
            : base(message)
        {
        }

        public RsaAesHybridException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
