using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    /// <summary>Decorates the async disposable created by SubscribeAsync() so the caller can obtain the generated
    /// subscription Id and any errors that occurred during connection / registration.</summary>
    internal sealed class AppSubscriptionRegistration : IAppSubscriptionRegistration
    {
        // The subscription registration being decorated. When disposed, the registration is unregistered from AppSync.
        private IAsyncDisposable _disposable;

        /// <summary>The subscription Id.</summary>
        public string Id { get; }

        /// <summary>When not null, contains exceptions raised during registration, such as connection issues.</summary>
        public IReadOnlyCollection<Exception> Exceptions { get; }

        /// <summary>When not null, contains graphql errors raised during registration, such as request/response mapping errors.</summary>
        public IReadOnlyCollection<GraphqlErrorDetail> GraphqlErrors { get; }

        /// <summary>Returns true if the registration was completed successfully (no graphql errors or exceptions).</summary>
        public bool Success => _disposable != null;     // Same as: Exceptions == null && GraphqlErrors == null;

        /// <summary>Constructor. Used when a successful subscription is registered with AppSync.</summary>
        /// <param name="id">The subscription identifier.</param>
        /// <param name="disposable">The subscription that will unregister from AppSync when disposed.</param>
        public AppSubscriptionRegistration(string id, IAsyncDisposable disposable)
        {
            Id = id.WhenNotNullOrEmpty(nameof(id));
            _disposable = disposable.WhenNotNull(nameof(disposable));
        }

        /// <summary>Constructor.</summary>
        /// <param name="id">The subscription identifier.</param>
        /// <param name="exceptions">One or more exceptions raised during the registration process. These exceptions
        /// are typically associated with connectivity issues.</param>
        public AppSubscriptionRegistration(string id, IEnumerable<Exception> exceptions)
        {
            Id = id.WhenNotNullOrEmpty(nameof(id));
            Exceptions = exceptions
                .WhenNotNullOrEmpty(nameof(exceptions))
                .AsReadOnlyCollection();
        }

        // Used when a connection is available but the subscription fails due to one or more graphql errors
        public AppSubscriptionRegistration(string id, IEnumerable<GraphqlErrorDetail> errors)
        {
            Id = id.WhenNotNullOrEmpty(nameof(id));
            GraphqlErrors = errors.AsReadOnlyCollection();
        }

        public async ValueTask DisposeAsync()
        {
            // Unsubscribe from AppSync
            if (_disposable != null)
            {
                await _disposable.DisposeAsync();
                _disposable = null;
            }
        }
    }
}