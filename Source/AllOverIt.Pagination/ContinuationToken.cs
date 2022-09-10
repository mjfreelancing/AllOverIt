using System.Collections.Generic;

namespace AllOverIt.Pagination
{
    internal sealed class ContinuationToken : IContinuationToken
    {
        public static readonly ContinuationToken None = new();

        /// <Inheritdoc />
        public PaginationDirection Direction { get; init; }

        /// <Inheritdoc />
        public IReadOnlyCollection<object> Values { get; init; }
    }
}
