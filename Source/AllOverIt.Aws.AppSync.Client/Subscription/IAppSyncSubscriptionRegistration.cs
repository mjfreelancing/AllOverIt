using AllOverIt.Aws.AppSync.Client.Response;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    /// <summary>Contains details of the success or failure of a subscription registration request.</summary>
    public interface IAppSyncSubscriptionRegistration : IAsyncDisposable
    {
        /// <summary>The unique subscription identifier.</summary>
        string Id { get; }

        /// <summary>When not null, contains one or more exceptions raised during the request to register the subscription.</summary>
        Exception[]? Exceptions { get; }

        /// <summary>When not null, contains one or more graphql errors received during the request to register the subscription.</summary>
        GraphqlErrorDetail[]? GraphqlErrors { get; }

        /// <summary>Indicates if the subscription was successfully registered with AppSync.</summary>
        bool Success { get; }
    }
}