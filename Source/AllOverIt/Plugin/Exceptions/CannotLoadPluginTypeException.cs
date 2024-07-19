namespace AllOverIt.Plugin.Exceptions
{
    /// <summary>Represents an error raised when attempting to create a type from a loaded plugin assembly.</summary>
    public sealed class CannotLoadPluginTypeException : Exception
    {
        /// <summary>Default constructor.</summary>
        public CannotLoadPluginTypeException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public CannotLoadPluginTypeException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CannotLoadPluginTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
