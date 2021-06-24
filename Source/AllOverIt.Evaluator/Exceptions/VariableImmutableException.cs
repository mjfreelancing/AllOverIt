using System;
using System.Runtime.Serialization;

namespace AllOverIt.Evaluator.Exceptions
{
    // An exception that can be thrown to indicate a concrete IVariable instance is not mutable.
    [Serializable]
    public class VariableImmutableException : VariableException
    {
        public VariableImmutableException()
        {
        }

        public VariableImmutableException(string message)
            : base(message)
        {
        }

        public VariableImmutableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected VariableImmutableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
