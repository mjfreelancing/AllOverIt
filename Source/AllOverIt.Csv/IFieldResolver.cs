using System.Collections.Generic;

namespace AllOverIt.Csv
{
    /// <summary>Represents a resolver that provides one or more column names and associated values for a particular field.</summary>
    /// <typeparam name="TCsvData">The data type being serialized to CSV format.</typeparam>
    public interface IFieldResolver<in TCsvData>
    {
        /// <summary>One or more column names to be exported.</summary>
        IReadOnlyCollection<string> HeaderNames { get; }

        /// <summary>Provides the values to be exported for each column name.</summary>
        /// <param name="data">The data instance to provide the values to be exported.</param>
        /// <returns>The values to be exported for each column name.</returns>
        IReadOnlyCollection<object> GetValues(TCsvData data);
    }
}
