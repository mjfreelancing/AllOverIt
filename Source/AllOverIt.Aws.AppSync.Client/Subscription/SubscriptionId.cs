using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    // Decorates the async disposable created by SubscribeAsync() so the caller can obtain the generated subscription Id.
    public sealed class SubscriptionId : IAsyncDisposable
    {
        private IAsyncDisposable _disposable;

        public string Id { get; }
        public IReadOnlyCollection<Exception> Exceptions { get; }
        public IReadOnlyCollection<GraphqlErrorDetail> GraphqlErrors { get; }
        public bool Success => Exceptions == null && GraphqlErrors == null;

        // Used when the subscription was registered successfully
        public SubscriptionId(string id, IAsyncDisposable disposable)
        {
            Id = id.WhenNotNullOrEmpty(nameof(id));
            _disposable = disposable.WhenNotNull(nameof(disposable));
        }

        // Used when a connection cannot be established (no registration Id)
        public SubscriptionId(IEnumerable<Exception> exceptions)
        {
            Exceptions = exceptions
                .WhenNotNullOrEmpty(nameof(exceptions))
                .AsReadOnlyCollection();
        }

        // Used when a connection is available but the subscription fails due to one or more exceptions (such as connection issues)
        public SubscriptionId(string id, IEnumerable<Exception> exceptions)
            : this(exceptions)
        {
            Id = id.WhenNotNullOrEmpty(nameof(id));
        }

        // Used when a connection is available but the subscription fails due to one or more graphql errors
        public SubscriptionId(string id, IEnumerable<GraphqlErrorDetail> errors)
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