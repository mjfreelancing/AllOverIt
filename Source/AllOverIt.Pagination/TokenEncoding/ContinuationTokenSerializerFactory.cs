using AllOverIt.Assertion;

namespace AllOverIt.Pagination.TokenEncoding
{
    internal sealed class ContinuationTokenSerializerFactory : IContinuationTokenSerializerFactory
    {
        public IContinuationTokenSerializer CreateContinuationTokenSerializer(IContinuationTokenOptions continuationTokenOptions)
        {
            _ = continuationTokenOptions.WhenNotNull();

            return new ContinuationTokenSerializer(continuationTokenOptions);
        }
    }
}
