using System;

namespace AllOverIt.Filtering.Exceptions
{
    /// <summary>An exception that can be thrown wihle building a query filter.</summary>
    public class NullNotSupportedException : Exception
    {
        /// <summary>Constructor.</summary>
        internal NullNotSupportedException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        internal NullNotSupportedException(string message)
            : base(message)
        {
        }
    }
}
