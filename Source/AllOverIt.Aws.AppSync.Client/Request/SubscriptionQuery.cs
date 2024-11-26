namespace AllOverIt.Aws.AppSync.Client.Request
{
    /// <summary>A subscription query with a unique identifier and optional variables associated with the query.</summary>
    public sealed class SubscriptionQuery
    {
        /// <summary>The unique subscription identifier.</summary>
        public string Id { get; }

        /// <summary> The subscription query.</summary>
        public required string Query { get; set; }

        /// <summary>An object (can be anonymous) that contains variables referenced by the query.</summary>
        public object? Variables { get; set; }

        /// <summary>Constructor. Sets a new, unique, identifier for the subscription.</summary>
        public SubscriptionQuery()
            : this(Guid.NewGuid())
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="id">The unique subscription identifier to use.</param>
        public SubscriptionQuery(Guid id)
        {
            Id = $"{id:N}";
        }
    }
}