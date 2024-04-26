using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Aws.AppSync.Client.Request;
using AllOverIt.Aws.AppSync.Client.Response;

namespace AllOverIt.Aws.AppSync.Client
{
    /// <summary>Represents an AppSync query and mutation graphql client.</summary>
    public interface IAppSyncClient
    {
        /// <summary>Sends a graphql query request using the default authorization specified on the configuration
        /// provided at the time of construction.</summary>
        /// <typeparam name="TResponse">The query response type.</typeparam>
        /// <param name="query">The query request to send.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>The query response, or error information.</returns>
        Task<GraphqlHttpResponse<TResponse>> SendQueryAsync<TResponse>(GraphqlQuery query, CancellationToken cancellationToken = default);

        /// <summary>Sends a graphql query request.</summary>
        /// <typeparam name="TResponse">The query response type.</typeparam>
        /// <param name="query">The query request to send.</param>
        /// <param name="authorization">The authorization to use for the request.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>The query response, or error information.</returns>
        Task<GraphqlHttpResponse<TResponse>> SendQueryAsync<TResponse>(GraphqlQuery query, IAppSyncAuthorization authorization,
            CancellationToken cancellationToken = default);

        /// <summary>Sends a graphql mutation request using the default authorization specified on the configuration
        /// provided at the time of construction.</summary>
        /// <typeparam name="TResponse">The mutation response type.</typeparam>
        /// <param name="query">The mutation request to send.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>The mutation response, or error information.</returns>
        Task<GraphqlHttpResponse<TResponse>> SendMutationAsync<TResponse>(GraphqlQuery query, CancellationToken cancellationToken = default);

        /// <summary>Sends a graphql mutation request.</summary>
        /// <typeparam name="TResponse">The mutation response type.</typeparam>
        /// <param name="query">The mutation request to send.</param>
        /// <param name="authorization">The authorization to use for the request.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>The mutation response, or error information.</returns>
        Task<GraphqlHttpResponse<TResponse>> SendMutationAsync<TResponse>(GraphqlQuery query, IAppSyncAuthorization authorization,
            CancellationToken cancellationToken = default);
    }
}