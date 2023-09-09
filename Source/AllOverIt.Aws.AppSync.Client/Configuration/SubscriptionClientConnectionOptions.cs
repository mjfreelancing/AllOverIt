using System;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    /// <summary>Provides subscription client connection options.</summary>
    public sealed class SubscriptionClientConnectionOptions
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

        /// <summary>The initial connection-handshake timeout period.</summary>
        public TimeSpan ConnectionTimeout { get; init; } = DefaultTimeout;

        /// <summary>The initial subscribe-handshake and unsubscribe-handshake timeout period. If a subscribe fails
        /// then the subscription is returned in an error state. If an unsubscribe fails, the subscription is dropped
        /// on the assumption there is something wrong with the connection.</summary>
        public TimeSpan SubscriptionTimeout { get; init; } = DefaultTimeout;
    }
}