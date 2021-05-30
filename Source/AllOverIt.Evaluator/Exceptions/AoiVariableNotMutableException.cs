using System;
using System.Runtime.Serialization;

namespace AllOverIt.Evaluator.Exceptions
{
    // An exception that can be thrown to indicate a concrete IAoiVariable instance is not mutable.
    [Serializable]
    public class AoiVariableNotMutableException : AoiVariableException
    {
        public AoiVariableNotMutableException()
        {
        }

        public AoiVariableNotMutableException(string message)
            : base(message)
        {
        }

        public AoiVariableNotMutableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AoiVariableNotMutableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
