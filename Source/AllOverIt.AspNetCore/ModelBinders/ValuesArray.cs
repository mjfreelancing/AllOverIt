using System.Collections.Generic;

namespace AllOverIt.AspNetCore.ModelBinders
{
    // Only applicable for models used to bind from a query string
    public abstract record ValuesArray<TType>
    {
        public IReadOnlyCollection<TType> Values { get; init; }
    }
}
