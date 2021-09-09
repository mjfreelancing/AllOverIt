using System;

namespace AllOverIt.Aws.AppSync.Client.Subscription.Configuration
{
    public sealed class AppSyncClientConnectionOptions
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(3);

        public TimeSpan ConnectionTimeout { get; } = DefaultTimeout;

        /// <summary>Initial subscribe and unsubscribe timeout period. If a subscribe fails the subscription is
        /// returned in an error state. If an unsubscribe fails, the subscription is dropped on the assumption
        /// there is something wrong with the connection.</summary>
        public TimeSpan SubscriptionTimeout { get; } = DefaultTimeout;
    }
}