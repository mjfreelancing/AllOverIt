using System;

namespace AllOverIt.Exceptions
{
    /// <summary>Represents an error that occurred while executing a command.</summary>
    public class CommandException : Exception
    {
        /// <summary>Default constructor.</summary>
        public CommandException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public CommandException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CommandException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}