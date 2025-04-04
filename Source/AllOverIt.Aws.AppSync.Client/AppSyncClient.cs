﻿using AllOverIt.Assertion;
using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Aws.AppSync.Client.Configuration;
using AllOverIt.Aws.AppSync.Client.Exceptions;
using AllOverIt.Aws.AppSync.Client.Request;
using AllOverIt.Aws.AppSync.Client.Response;
using System.Text;

namespace AllOverIt.Aws.AppSync.Client
{
    /// <summary>An AppSync query and mutation graphql client.</summary>
    public sealed class AppSyncClient : IAppSyncClient
    {
        /// <summary>The expected <see cref="HttpClient"/> name to be registered with <see cref="IHttpClientFactory"/>.</summary>
        public static readonly string HttpClientName = typeof(AppSyncClient).FullName!;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAppSyncClientConfiguration _configuration;

        /// <summary>Constructor.</summary>
        /// <param name="httpClientFactory">The <see cref="HttpClient"/> factory used to get a named client for sending requests. The client
        /// must be registered with the name provided by <see cref="HttpClientName"/>.</param>
        /// <param name="configuration">Contains configuration details for AppSync Graphql query and mutation operations.</param>
        public AppSyncClient(IHttpClientFactory httpClientFactory, IAppSyncClientConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory.WhenNotNull();
            _configuration = configuration.WhenNotNull();
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

        private async Task<GraphqlHttpResponse<TResponse>> SendRequestAsync<TResponse>(GraphqlQuery query, IAppSyncAuthorization? authorization,
            CancellationToken cancellationToken)
        {
            using (var requestMessage = CreateHttpRequestMessage(query, authorization))
            {
                using (var responseMessage = await GetHttpResponseMessageAsync(requestMessage, cancellationToken).ConfigureAwait(false))
                {
                    var content = await responseMessage.Content
                        .ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false);

                    var result = _configuration.Serializer.DeserializeObject<GraphqlHttpResponse<TResponse>>(content)!;

                    result.StatusCode = responseMessage.StatusCode;
                    result.Headers = responseMessage.Headers;

                    if (responseMessage.IsSuccessStatusCode && result.Errors is null)
                    {
                        return result;
                    }

                    throw new GraphqlHttpRequestException(responseMessage.StatusCode, result.Errors, content);
                }
            }
        }

        private HttpRequestMessage CreateHttpRequestMessage(GraphqlQuery query, IAppSyncAuthorization? authorization)
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

        private async Task<HttpResponseMessage> GetHttpResponseMessageAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient(HttpClientName);

            return await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
    }
}