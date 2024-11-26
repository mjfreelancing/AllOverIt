using AllOverIt.Assertion;

namespace AllOverIt.Pagination.TokenEncoding
{
    internal sealed class ContinuationTokenEncoderFactory : IContinuationTokenEncoderFactory
    {
        private readonly IContinuationTokenSerializerFactory _serializerFactory;

        public ContinuationTokenEncoderFactory(IContinuationTokenSerializerFactory serializerFactory)
        {
            _serializerFactory = serializerFactory.WhenNotNull();
        }

        public IContinuationTokenEncoder CreateContinuationTokenEncoder(IReadOnlyCollection<IColumnDefinition> columns, PaginationDirection paginationDirection,
            ContinuationTokenOptions continuationTokenOptions)
        {
            _ = columns.WhenNotNullOrEmpty();
            _ = continuationTokenOptions.WhenNotNull();

            var tokenSerializer = _serializerFactory.CreateContinuationTokenSerializer(continuationTokenOptions);

            return new ContinuationTokenEncoder(columns, paginationDirection, tokenSerializer);
        }
    }
}
