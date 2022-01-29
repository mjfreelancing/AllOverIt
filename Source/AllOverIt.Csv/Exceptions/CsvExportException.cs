using System;

namespace AllOverIt.Csv.Exceptions
{
    /// <summary>Represents an error that occurs during the export of data to CSV.</summary>
    public sealed class CsvExportException : Exception
    {
        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public CsvExportException(string message)
            : base(message)
        {
        }
    }
}