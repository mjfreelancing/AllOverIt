using System;
using System.Runtime.Serialization;

namespace AllOverIt.Evaluator.Exceptions
{
    // An exception that can be thrown by a concrete IAoiVariable.
    [Serializable]
    public class AoiVariableException : Exception
    {
        public AoiVariableException()
        {
        }

        public AoiVariableException(string message)
            : base(message)
        {
        }

        public AoiVariableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AoiVariableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
