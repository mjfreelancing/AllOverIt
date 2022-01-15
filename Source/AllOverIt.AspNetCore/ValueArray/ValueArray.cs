using System.Collections.Generic;

namespace AllOverIt.AspNetCore.ValueArray
{
    /// <summary>Represents an array of values that can be bound to a model from a query string.</summary>
    /// <remarks>The expected format is paramName=Value1,Value2,Value3 with each value quoted if required.</remarks>
    public abstract record ValueArray<TType>
    {
        /// <summary>The values converted from the query string.</summary>
        public IReadOnlyCollection<TType> Values { get; init; }
    }
}
