using AllOverIt.Helpers;
using System;
using System.Threading.Tasks;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    // Decorates the async disposable created by SubscribeAsync() so the caller can obtain the generated subscription Id.
    public sealed class SubscriptionId : IAsyncDisposable
    {
        private readonly IAsyncDisposable _disposable;

        public string Id { get; }

        public SubscriptionId(string id, IAsyncDisposable disposable)
        {
            Id = id.WhenNotNullOrEmpty(nameof(id));
            _disposable = disposable.WhenNotNull(nameof(disposable));
        }

        public ValueTask DisposeAsync()
        {
            // Unregisters the subscription from AppSync
            return _disposable.DisposeAsync();
        }
    }
}