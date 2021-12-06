using AllOverIt.Assertion;
using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Aws.AppSync.Client.Configuration;
using AllOverIt.Aws.AppSync.Client.Exceptions;
using AllOverIt.Aws.AppSync.Client.Request;
using AllOverIt.Aws.AppSync.Client.Response;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Aws.AppSync.Client
{
    /// <summary>An AppSync query and mutation graphql client.</summary>
    public sealed class AppSyncClient : IAppSyncClient
    {
        private static readonly HttpClient HttpClient = new();
        private readonly IGraphqlClientConfiguration _configuration;

        /// <summary>Constructor.</summary>
        /// <param name="configuration">Contains configuration details for AppSync Graphql query and mutation operations.</param>
        public AppSyncClient(IGraphqlClientConfiguration configuration)
        {
            _configuration = configuration.WhenNotNull(nameof(configuration));
        }

        /// <inheritdoc />
        public Task<GraphqlHttpResponse<TResponse>> SendQueryAsync<TResponse>(GraphqlQuery query, CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<TResponse>(query, null, cancellationToken);
        }

        /// <inheritdoc />
        public Task<GraphqlHttpResponse<TResponse>> SendQueryAsync<TResponse>(GraphqlQuery query, IAppSyncAuthorization authorization,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<TResponse>(query, authorization, cancellationToken);
        }

        /// <inheritdoc />
        public Task<GraphqlHttpResponse<TResponse>> SendMutationAsync<TResponse>(GraphqlQuery query, CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<TResponse>(query, null, cancellationToken);
        }

        /// <inheritdoc />
        public Task<GraphqlHttpResponse<TResponse>> SendMutationAsync<TResponse>(GraphqlQuery query, IAppSyncAuthorization authorization,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<TResponse>(query, authorization, cancellationToken);
        }

        private async Task<GraphqlHttpResponse<TResponse>> SendRequestAsync<TResponse>(GraphqlQuery query, IAppSyncAuthorization authorization,
            CancellationToken cancellationToken)
        {
            using (var requestMessage = CreateHttpRequestMessage(query, authorization))
            {
                using (var responseMessage = await GetHttpResponseMessageAsync(requestMessage, cancellationToken).ConfigureAwait(false))
                {
                    var content = await responseMessage.Content
                        .ReadAsStringAsync()
                        .ConfigureAwait(false);

                    var result = _configuration.Serializer
                        .DeserializeObject<GraphqlHttpResponse<TResponse>>(content);

                    result.StatusCode = responseMessage.StatusCode;
                    result.Headers = responseMessage.Headers;

                    if (responseMessage.IsSuccessStatusCode && result.Errors == null)
                    {
                        return result;
                    }

                    throw new GraphqlHttpRequestException(responseMessage.StatusCode, result.Errors, content);
                }
            }
        }

        private HttpRequestMessage CreateHttpRequestMessage(GraphqlQuery query, IAppSyncAuthorization authorization)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, _configuration.EndPoint)
            {
                Content = new StringContent(_configuration.Serializer.SerializeObject(query), Encoding.UTF8, "application/graphql")
            };

            authorization ??= _configuration.DefaultAuthorization;

            foreach (var (key, value) in authorization.KeyValues)
            {
                message.Headers.Add(key, value);
            }

            return message;
        }

        private static Task<HttpResponseMessage> GetHttpResponseMessageAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            return HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
    }
}