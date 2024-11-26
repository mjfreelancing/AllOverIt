namespace AllOverIt.Aws.AppSync.Client.Request
{
    /// <summary>A graphql query with optional variables associated with the query.</summary>
    public sealed class GraphqlQuery
    {
        /// <summary> The graphql query.</summary>
        public required string Query { get; set; }

        /// <summary>An object (can be anonymous) that contains variables referenced by the query.</summary>
        public object? Variables { get; set; }
    }
}