namespace AllOverIt.Pagination
{
    public static class ContinuationTokenEncoderExtensions
    {
        public static string Encode(this IContinuationTokenEncoder encoder, IContinuationToken continuationToken)
        {
            return encoder.Serializer.Serialize(continuationToken);
        }

        public static IContinuationToken Decode(this IContinuationTokenEncoder encoder, string continuationToken)
        {
            return encoder.Serializer.Deserialize(continuationToken);
        }
    }
}
