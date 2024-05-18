using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Aws.AppSync.Client.Request;
using AllOverIt.Aws.AppSync.Client.Response;
using AllOverIt.Aws.AppSync.Client.Subscription;

namespace AllOverIt.Aws.AppSync.Client
{
    /// <summary>Represents an AppSync subscription client that supports API KEY and Cognito based authorization.</summary>
    public interface IAppSyncSubscriptionClient
    {
        /// <summary>An observable providing the current connection status.</summary>
        IObservable<SubscriptionConnectionState> ConnectionState { get; }

        /// <summary>An observable reporting connection and subscription related exceptions.</summary>
        IObservable<Exception> Exceptions { get; }

        /// <summary>An observable reporting subscription related graphql errors.</summary>
        IObservable<GraphqlSubscriptionResponseError> GraphqlErrors { get; }

        /// <summary>Indicates if the client is currently connected to AppSync.</summary>
        bool IsAlive { get; }

        /// <summary>Opens a WebSocket connection and registers the client with AppSync. Any existing subscriptions from a
        /// previous connection will be re-subscribed.</summary>
        /// <param name="authorization">The authorization to use for the request. If <see langword="null"/> is provided then the default authorization provided
        /// on the client configuration during construction will be used.</param>
        /// <returns><see langword="True" /> if the connection was established, otherwise <see langword="False" />.</returns>
        /// <remarks>Refer to <see cref="DisconnectAsync"/> for more information on how existing subscriptions are retained
        /// if the client is disconnected while there are active subscriptions.</remarks>
        Task<bool> ConnectAsync(IAppSyncAuthorization? authorization = default);

        /// <summary>Unsubscribes any existing subscriptions and then disconnects the client from AppSync. The subscription
        /// registrations are maintained (not disposed of). If a new connection is later established by calling
        /// <see cref="ConnectAsync"/> or making a new subscription then all previous subscriptions will be re-subscribed.</summary>
        Task DisconnectAsync();

        /// <summary>Registers a new subscription with AppSync. If there is no active connection then that will be established first.</summary>
        /// <typeparam name="TResponse">The response type to be populated when the subscription receives a message.</typeparam>
        /// <param name="query">The subscription query.</param>
        /// <param name="responseAction">The action to invoke when a response is received.</param>
        /// <param name="authorization">The authorization to use for the request. If <see langword="null"/> is provided then the default authorization provided
        /// on the client configuration during construction will be used.</param>
        /// <returns>A subscription registration. The subscription will be closed when this registration is disposed of.</returns>
        Task<IAppSyncSubscriptionRegistration> SubscribeAsync<TResponse>(SubscriptionQuery query,
            Action<GraphqlSubscriptionResponse<TResponse>> responseAction, IAppSyncAuthorization? authorization = null);
    }
}
