using System.Collections.Generic;

namespace AllOverIt.Csv
{
    public sealed class HeaderIdentifier<THeaderId>
    {
        public THeaderId Id { get; init; }      // Could be the item's index within a collection or a key in a dictionary
        public IReadOnlyCollection<string> Names { get; init; }
    }
}
