namespace AllOverIt.Aws.AppSync.Client.Authorization
{
    /// <summary>Represents AppSync authorization header key-value pairs.</summary>
    public interface IAppSyncAuthorization
    {
        /// <summary>Contains authorization header key-value pairs.</summary>
        IDictionary<string, string> KeyValues { get; }
    }
}