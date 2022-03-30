using AllOverIt.Aws.AppSync.Client.Configuration;

namespace AllOverIt.Aws.AppSync.Client
{
    internal delegate IAppSyncClientConfiguration NamedAppSyncClientConfigurationDelegate(string name);
    internal delegate IAppSyncClient NamedAppSyncClientDelegate(string name);

    /// <summary>Represents a provider that returns an implementation of <see cref="IAppSyncClient"/> for a specified name.</summary>
    public interface IAppSyncNamedClientProvider
    {
        /// <summary>Gets an <see cref="IAppSyncClient"/> instance specific to the provided name.</summary>
        /// <param name="name">The name of the client instance to retrieve.</param>
        /// <returns>An <see cref="IAppSyncClient"/> instance specific to the provided name.</returns>
        IAppSyncClient GetClient(string name);
    }
}