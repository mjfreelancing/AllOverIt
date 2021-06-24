using System;
using System.Runtime.Serialization;

namespace AllOverIt.Evaluator.Exceptions
{
    // An exception that can be thrown by a concrete IAoiOperator while compiling a formula expression.
    [Serializable]
    public class OperatorException : Exception
    {
        public OperatorException()
        {
        }

        public OperatorException(string message)
            : base(message)
        {
        }

        public OperatorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected OperatorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
