using System;

namespace AllOverIt.Exceptions
{
    public class SelfReferenceException : Exception
    {
        public SelfReferenceException()
        {
        }

        public SelfReferenceException(string message)
            : base(message)
        {
        }

        public SelfReferenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
