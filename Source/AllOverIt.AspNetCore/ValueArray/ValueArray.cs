using System.Collections.Generic;

namespace AllOverIt.AspNetCore.ValueArray
{
    // Only applicable for models used to bind from a query string
    public abstract record ValueArray<TType>
    {
        public IReadOnlyCollection<TType> Values { get; init; }
    }
}
