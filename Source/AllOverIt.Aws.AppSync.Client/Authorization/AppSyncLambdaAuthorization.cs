namespace AllOverIt.Aws.AppSync.Client.Authorization
{
    /// <summary>Contains AppSync Lambda authorization header key-value pairs.</summary>
    public sealed class AppSyncLambdaAuthorization : AppSyncAuthorizationBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="value">The authorization value.</param>
        public AppSyncLambdaAuthorization(string value)
        {
            KeyValues.Add("authorization", value);
        }
    }
}