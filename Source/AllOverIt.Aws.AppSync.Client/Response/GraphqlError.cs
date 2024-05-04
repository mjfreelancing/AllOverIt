namespace AllOverIt.Aws.AppSync.Client.Response
{
    /// <summary>Contains graphql errors reported by AppSync.</summary>
    public sealed class GraphqlError
    {
        /// <summary>One or more detailed graphql errors reported by AppSync.</summary>
        public IEnumerable<GraphqlErrorDetail> Errors { get; init; }
    }
}