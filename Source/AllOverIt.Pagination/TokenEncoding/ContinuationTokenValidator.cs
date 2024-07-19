using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.Pagination.TokenEncoding
{
    /// <summary>Validates a serialized continuation token.</summary>
    public sealed class ContinuationTokenValidator : IContinuationTokenValidator
    {
        private readonly IContinuationTokenSerializerFactory _serializerFactory;

        /// <summary>Constructor.</summary>
        /// <param name="serializerFactory">A factory to create a continuation token serializer that can validate a serialized token.</param>
        public ContinuationTokenValidator(IContinuationTokenSerializerFactory serializerFactory)
        {
            _serializerFactory = serializerFactory.WhenNotNull(nameof(serializerFactory));
        }

        /// <inheritdoc />
        public bool IsValidToken(string? continuationToken, IContinuationTokenOptions tokenOptions)
        {
            _ = tokenOptions.WhenNotNull(nameof(tokenOptions));

            if (continuationToken.IsNullOrEmpty())
            {
                return true;
            }

            var serializer = _serializerFactory.CreateContinuationTokenSerializer(tokenOptions);

            return serializer.TryDeserialize(continuationToken, out _);
        }
    }
}
