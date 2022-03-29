using AllOverIt.Csv.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AllOverIt.Csv
{
    /// <summary>Represents a CSV serializer for a complex data type.</summary>
    /// <typeparam name="TCsvData">The complex data type to be exported.</typeparam>
    public interface ICsvSerializer<TCsvData>
    {
        /// <summary>Configures a field for export.</summary>
        /// <param name="headerName">The column header name to be exported.</param>
        /// <param name="valueResolver">Returns the value to be exported for the current field.</param>
        void AddField(string headerName, Func<TCsvData, object> valueResolver);

        /// <summary>Configures a field to export multiple property values with associated column header names.</summary>
        /// <param name="headerNames">The header names to export.</param>
        /// <param name="valuesResolver">A Func to return a value for each of the configured header names.</param>
        /// <remarks>Also refer to <see cref="CsvSerializerExtensions.AddDynamicFields{TCsvData, TField}"/> and
        /// <see cref="CsvSerializerExtensions.AddDynamicFields{TCsvData, TField, TFieldId}"/> for an alternative, and
        /// potentially easier, approach to adding multiple columns from a complex property.</remarks>
        void AddFields(IEnumerable<string> headerNames, Func<TCsvData, IEnumerable<object>> valuesResolver);

        /// <summary>Serializes data to CSV format.</summary>
        /// <param name="writer">Writes the exported data to CSV format.</param>
        /// <param name="data">The data to be exported.</param>
        /// <param name="includeHeader">Indicates if the header names are to be exported.</param>
        /// <returns>A task that completes when the export is completed.</returns>
        Task SerializeAsync(TextWriter writer, IEnumerable<TCsvData> data, bool includeHeader = true);
    }
}
