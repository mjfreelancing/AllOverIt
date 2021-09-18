using System;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    public sealed record SubscriptionClientConnectionOptions
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

        public TimeSpan ConnectionTimeout { get; init; } = DefaultTimeout;

        /// <summary>Initial subscribe and unsubscribe timeout period. If a subscribe fails the subscription is
        /// returned in an error state. If an unsubscribe fails, the subscription is dropped on the assumption
        /// there is something wrong with the connection.</summary>
        public TimeSpan SubscriptionTimeout { get; init; } = DefaultTimeout;
    }
}