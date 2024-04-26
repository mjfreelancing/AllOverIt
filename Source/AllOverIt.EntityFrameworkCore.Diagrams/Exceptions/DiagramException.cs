namespace AllOverIt.EntityFrameworkCore.Diagrams.Exceptions
{
    /// <summary>Represents an error that occurs while generating an Entity Relationship Diagram.</summary>
    public class DiagramException : Exception
    {
        /// <summary>Default constructor.</summary>
        public DiagramException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public DiagramException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DiagramException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}