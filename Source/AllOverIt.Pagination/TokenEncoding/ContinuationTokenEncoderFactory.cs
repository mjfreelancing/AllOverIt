using AllOverIt.Assertion;
using System.Collections.Generic;

namespace AllOverIt.Pagination.TokenEncoding
{
    internal sealed class ContinuationTokenEncoderFactory : IContinuationTokenEncoderFactory
    {
        private readonly IContinuationTokenSerializerFactory _serializerFactory;

        public ContinuationTokenEncoderFactory(IContinuationTokenSerializerFactory serializerFactory)
        {
            _serializerFactory = serializerFactory.WhenNotNull(nameof(serializerFactory));
        }

        public IContinuationTokenEncoder CreateContinuationTokenEncoder(IReadOnlyCollection<IColumnDefinition> columns, PaginationDirection paginationDirection,
            ContinuationTokenOptions continuationTokenOptions)
        {
            _ = columns.WhenNotNullOrEmpty(nameof(columns));
            _ = continuationTokenOptions.WhenNotNull(nameof(continuationTokenOptions));

            var tokenSerializer = _serializerFactory.CreateContinuationTokenSerializer(continuationTokenOptions);

            return new ContinuationTokenEncoder(columns, paginationDirection, tokenSerializer);
        }
    }
}
