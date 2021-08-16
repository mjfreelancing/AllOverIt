using System;
using System.Runtime.Serialization;

namespace AllOverIt.Evaluator.Exceptions
{
    /// <summary>An exception that can be thrown to indicate a concrete IVariable instance is not mutable.</summary>
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
