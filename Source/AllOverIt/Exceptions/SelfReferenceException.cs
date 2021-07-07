using System;
using System.Runtime.Serialization;

namespace AllOverIt.Exceptions
{
    [Serializable]
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

        protected SelfReferenceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
