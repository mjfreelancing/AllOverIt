using System;

namespace AllOverIt.Csv.Exceptions
{
    /// <summary>Represents an error that occurs during the export of data to CSV.</summary>
    public sealed class CsvExporterException : Exception
    {
        /// <summary>Default constructor.</summary>
        public CsvExporterException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public CsvExporterException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CsvExporterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}