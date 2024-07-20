using AllOverIt.Assertion;
using System.Collections.Concurrent;

namespace AllOverIt.Aws.AppSync.Client
{
    internal sealed class AppSyncNamedClientProvider : IAppSyncNamedClientProvider
    {
        private readonly NamedAppSyncClientDelegate _namedClient;
        private readonly ConcurrentDictionary<string, IAppSyncClient> _namedClients = new();

        /// <summary>Constructor.</summary>
        /// <param name="namedClientDelegate">A delegate responsible for obtaining an AppSync client by name.</param>
        public AppSyncNamedClientProvider(NamedAppSyncClientDelegate namedClientDelegate)
        {
            _namedClient = namedClientDelegate.WhenNotNull();
        }

        /// <inheritdoc />
        public IAppSyncClient GetClient(string name)
        {
            return _namedClients.GetOrAdd(name, clientName => _namedClient.Invoke(clientName));
        }
    }
}