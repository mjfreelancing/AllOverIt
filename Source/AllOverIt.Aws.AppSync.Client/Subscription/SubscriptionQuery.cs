using System;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class SubscriptionQuery
    {
        public string Id { get; }
        public string Query { get; set; }
        public object Variables { get; set; }

        public SubscriptionQuery(string id = default)
        {
            Id = id ?? $"{Guid.NewGuid():N}";
        }
    }
}