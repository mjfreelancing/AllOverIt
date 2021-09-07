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
        public IReadOnlyList<Exception> Exceptions { get; }
        public bool Success => Exceptions == null;

        // Used when the subscription was registered successfully
        public SubscriptionId(string id, IAsyncDisposable disposable)
        {
            Id = id.WhenNotNullOrEmpty(nameof(id));
            _disposable = disposable.WhenNotNull(nameof(disposable));
        }

        // Used when a connection cannot be established (no registration Id)
        public SubscriptionId(IReadOnlyList<Exception> exceptions)
        {
            // Not assigning to Exceptions - would require a Cast<>
            _ = exceptions.WhenNotNullOrEmpty(nameof(exceptions));

            Exceptions = exceptions;
        }

        // Used when a connection is available but the subscription fails
        public SubscriptionId(string id, IReadOnlyList<Exception> exceptions)
            : this(exceptions)
        {
            Id = id.WhenNotNullOrEmpty(nameof(id));
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