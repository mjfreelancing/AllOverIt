using System;
using System.Runtime.Serialization;

namespace AllOverIt.Evaluator.Exceptions
{
    // An exception that can be thrown by a concrete IVariable.
    [Serializable]
    public class VariableException : Exception
    {
        public VariableException()
        {
        }

        public VariableException(string message)
            : base(message)
        {
        }

        public VariableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected VariableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
