using AllOverIt.Assertion;
using System.Collections.Concurrent;

namespace AllOverIt.Aws.AppSync.Client
{
    internal sealed class NamedAppSyncClientProvider : INamedAppSyncClientProvider
    {
        private readonly NamedAppSyncClientDelegate _namedClient;
        private readonly ConcurrentDictionary<string, IAppSyncClient> _namedClients = new();

        /// <summary>Constructor.</summary>
        /// <param name="namedClientDelegate">A delegate responsible for obtaining an AppSync client by name.</param>
        public NamedAppSyncClientProvider(NamedAppSyncClientDelegate namedClientDelegate)
        {
            _namedClient = namedClientDelegate.WhenNotNull(nameof(namedClientDelegate));
        }

        /// <inheritdoc />
        public IAppSyncClient GetClient(string name)
        {
            return _namedClients.GetOrAdd(name, clientName =>
            {
                return _namedClient.Invoke(name);
            });
        }
    }
}