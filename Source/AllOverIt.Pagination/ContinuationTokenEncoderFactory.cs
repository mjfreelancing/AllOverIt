using AllOverIt.Assertion;
using System.Collections.Generic;

namespace AllOverIt.Pagination
{
    internal sealed class ContinuationTokenEncoderFactory : IContinuationTokenEncoderFactory
    {
        public IContinuationTokenEncoder CreateContinuationTokenEncoder(IReadOnlyCollection<IColumnDefinition> columns, PaginationDirection paginationDirection,
            ContinuationTokenOptions continuationTokenOptions)
        {
            _  = columns.WhenNotNullOrEmpty(nameof(columns));
            _ = continuationTokenOptions.WhenNotNull(nameof(continuationTokenOptions));

            var tokenSerializer = new ContinuationTokenSerializer(continuationTokenOptions);

            return new ContinuationTokenEncoder(columns, paginationDirection, tokenSerializer);
        }
    }
}
