using System;
using System.Runtime.Serialization;

namespace AllOverIt.Evaluator.Exceptions
{
    // An exception that can be thrown by an AoiArithmeticOperationFactory while compiling a formula expression.
    [Serializable]
    public class AoiOperationFactoryException : Exception
    {
        public AoiOperationFactoryException()
        {
        }

        public AoiOperationFactoryException(string message)
            : base(message)
        {
        }

        public AoiOperationFactoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AoiOperationFactoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
