using AllOverIt.Assertion;
using AllOverIt.Pagination.TokenEncoding;

namespace AllOverIt.Pagination.Extensions
{
    internal static class ContinuationTokenEncoderExtensions
    {
        public static string Encode(this IContinuationTokenEncoder encoder, IContinuationToken continuationToken)
        {
            _ = encoder.WhenNotNull();
            _ = continuationToken.WhenNotNull();

            return encoder.Serializer.Serialize(continuationToken);
        }

        public static IContinuationToken Decode(this IContinuationTokenEncoder encoder, string? continuationToken)
        {
            _ = encoder.WhenNotNull();

            // continuationToken can be null
            return encoder.Serializer.Deserialize(continuationToken);
        }
    }
}
