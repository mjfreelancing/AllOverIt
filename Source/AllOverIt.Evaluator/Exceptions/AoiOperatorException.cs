using System;
using System.Runtime.Serialization;

namespace AllOverIt.Evaluator.Exceptions
{
    // An exception that can be thrown by a concrete IAoiOperator while compiling a formula expression.
    [Serializable]
    public class AoiOperatorException : Exception
    {
        public AoiOperatorException()
        {
        }

        public AoiOperatorException(string message)
            : base(message)
        {
        }

        public AoiOperatorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AoiOperatorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
