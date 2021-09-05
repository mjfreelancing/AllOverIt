using AllOverIt.Aws.AppSync.Client.Exceptions;
using AllOverIt.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphqlRequestType = AllOverIt.Aws.AppSync.Client.Subscription.Constants.GraphqlRequestType;
using GraphqlResponseType = AllOverIt.Aws.AppSync.Client.Subscription.Constants.GraphqlResponseType;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class GraphqlSubscriptionResponseError
    {
        public string Id { get; }
        public WebSocketResponse<GraphqlError> Error { get; }

        public GraphqlSubscriptionResponseError(string id, WebSocketResponse<GraphqlError> error)
        {
            Id = id.WhenNotNullOrEmpty(nameof(id));
            Error = error.WhenNotNull(nameof(error));
        }
    }

    // Implemented as per the protocol described at:
    // https://docs.aws.amazon.com/appsync/latest/devguide/real-time-websocket-client.html

    public sealed class AppSyncSubscriptionClient
    {
        private readonly string _host;
        private readonly Uri _uri;
        private readonly IAuthorization _defaultAuthorization;
        private readonly IAppSyncClientSerializer _serializer;
        private readonly CancellationTokenSource _cts = new();
        private readonly ArraySegment<byte> _buffer = new(new byte[8192]);
        private readonly IDictionary<string, SubscriptionRegistration> _subscriptions = new ConcurrentDictionary<string, SubscriptionRegistration>();
        private IConnectableObservable<AppSyncGraphqlResponse> _incomingMessages;
        private IDisposable _incomingMessagesConnection;

        // One WebSocket connection can have multiple subscriptions (even with different authentication modes).
        private ClientWebSocket _webSocket;
        private readonly SemaphoreSlim _webSocketLock = new(1, 1);
        private readonly BehaviorSubject<SubscriptionConnectionState> _connectionStateSubject = new(SubscriptionConnectionState.Disconnected);
        private readonly Subject<Exception> _exceptionSubject = new();
        private readonly Subject<GraphqlSubscriptionResponseError> _graphqlErrorSubject = new();

        public IObservable<SubscriptionConnectionState> ConnectionState => _connectionStateSubject;
        public IObservable<Exception> Exceptions => _exceptionSubject;
        public IObservable<GraphqlSubscriptionResponseError> GraphqlErrors => _graphqlErrorSubject;


        // endpoint is the graphql endpoint (not realtime) without https, wss, or /graphql
        // e.g., example123abc.appsync-api.ap-southeast-2.amazonaws.com
        public AppSyncSubscriptionClient(string host, IAuthorization defaultAuthorization, IAppSyncClientSerializer serializer)
        {
            _host = host.WhenNotNullOrEmpty(nameof(host));

            var realtime = _host.Replace("appsync-api", "appsync-realtime-api");
            var hostAuth = new HostAuthorization(host, defaultAuthorization);
            var encodedHeader = hostAuth.GetEncodedHeader();

            _uri = new Uri($"wss://{realtime}/graphql?header={encodedHeader}&payload=e30=");



            _defaultAuthorization = defaultAuthorization.WhenNotNull(nameof(defaultAuthorization));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        // returns null if there was an internal exception, such as a connection error (would have been reported via observables)
        public Task<SubscriptionId> SubscribeAsync<TResponse>(SubscriptionQuery query, Action<SubscriptionResponse<TResponse>> responseAction)
        {
            return SubscribeAsync(query, responseAction, null);
        }

        // returns null if there was an internal exception, such as a connection error (would have been reported via observables)
        // or subscription error (such as an invalid query). The default authorization mode will be used if authorization is null.
        public async Task<SubscriptionId> SubscribeAsync<TResponse>(SubscriptionQuery query, Action<SubscriptionResponse<TResponse>> responseAction,
            IAuthorization authorization)
        {
            // Will connect (and wait for ACK) if required
            var connectionState = await CheckWebSocketConnection().ConfigureAwait(false);

            // Abort if the connection failed
            if (connectionState == SubscriptionConnectionState.Disconnected)
            {
                return null;
            }

            authorization ??= _defaultAuthorization;

            var hostAuthorization = new HostAuthorization(_host, authorization);

            var payload = new SubscriptionQueryPayload
            {
                Data = _serializer.SerializeObject(query),
                Extensions = new { authorization = hostAuthorization.KeyValues }
            };

            var registration = new SubscriptionRegistration<TResponse>(payload, responseAction);

            _subscriptions.Add(registration.Id, registration);

            var ackTask = await SendRegistration(registration).ConfigureAwait(false);

            // wait for the registration ACK
            var response = await ackTask;

            if (response.Type == GraphqlResponseType.Error)
            {
                return null;
            }

            // This is decorated by SubscriptionId to avoid leaking SubscriptionRegistration
            var disposable = new RaiiAsync<SubscriptionRegistration>(
                () => registration,
                async subscription =>
                {
                    await UnregisterSubscription(subscription.Id).ConfigureAwait(false);
                });

            return new SubscriptionId(registration.Id, disposable);
        }

        private async Task<SubscriptionConnectionState> CheckWebSocketConnection()
        {
            await _webSocketLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (_webSocket is { State: WebSocketState.Open })
                {
                    return SubscriptionConnectionState.Connected;
                }

                try
                {
                    // dispose if not currently in an Open state
                    DisposeWebSocketConnection();

                    _connectionStateSubject.OnNext(SubscriptionConnectionState.Connecting);

                    _webSocket = new ClientWebSocket();
                    _webSocket.Options.AddSubProtocol("graphql-ws");

                    if (_webSocket.State != WebSocketState.Open)
                    {
                        await _webSocket.ConnectAsync(_uri, _cts.Token).ConfigureAwait(false);

                        ConfigureMessageProcessing();

                        var ack = _incomingMessages
                            .TakeUntil(response => response is { Type: GraphqlResponseType.ConnectionAck or GraphqlResponseType.ConnectionError })
                            .LastAsync()
                            .ToTask();

                        await SendInitRequest().ConfigureAwait(false);

                        var response = await ack.ConfigureAwait(false);

                        if (response.Type == GraphqlResponseType.ConnectionAck)
                        {
                            _connectionStateSubject.OnNext(SubscriptionConnectionState.Connected);

                            // re-register any existing subscriptions
                            await SendRegistrationRequests().ConfigureAwait(false);
                        }
                        else
                        {
                            var error = JsonConvert.DeserializeObject<WebSocketResponse<GraphqlError>>(response.Message);
                            throw new GraphqlConnectionException(error);      // the maintenance subscription will disconnect the web socket
                        }
                    }
                }
                catch (Exception exception)
                {
                    _exceptionSubject.OnNext(exception);
                    ShutdownConnection();
                }

                return _connectionStateSubject.Value;
            }
            finally
            {
                _webSocketLock.Release();
            }
        }

        private void ShutdownConnection()
        {
            // disposes of all subscriptions
            _incomingMessagesConnection?.Dispose();
            _incomingMessagesConnection = null;

            DisposeWebSocketConnection();

            _connectionStateSubject.OnNext(SubscriptionConnectionState.Disconnected);
        }

        private void DisposeWebSocketConnection()
        {
            // attempting to close the connection fails if the internal stream has been closed due to an error - disposing it is fine
            _webSocket?.Dispose();
            _webSocket = null;
        }

        private SubscriptionRegistration GetSubscription(string id)
        {
            if (_subscriptions.TryGetValue(id, out var subscription))
            {
                return subscription;
            }

            throw new KeyNotFoundException($"Subscription Id '{id}' not found.");
        }

        private void ConfigureMessageProcessing()
        {
            _incomingMessages = Observable
                .Defer(() => Task.Run(async () =>
                {
                    using (var ms = new MemoryStream())
                    {
                        WebSocketReceiveResult webSocketReceiveResult;

                        do
                        {
                            // WebSocketException is reported via maintenanceSubscription
                            webSocketReceiveResult = await _webSocket.ReceiveAsync(_buffer, _cts.Token).ConfigureAwait(false);
                            ms.Write(_buffer.Array!, _buffer.Offset, webSocketReceiveResult.Count);
                        } while (!webSocketReceiveResult.EndOfMessage && !_cts.Token.IsCancellationRequested);

                        _cts.Token.ThrowIfCancellationRequested();

                        ms.Seek(0, SeekOrigin.Begin);

                        switch (webSocketReceiveResult.MessageType)
                        {
                            case WebSocketMessageType.Text:
                            case WebSocketMessageType.Close:
                                return GetAppSyncGraphqlResponse(ms);

                            default:
                                throw new InvalidOperationException(
                                    $"Unexpected websocket message type '{webSocketReceiveResult.MessageType}'.");
                        }
                    }
                }).ToObservable())
                .Repeat()
                .Catch<AppSyncGraphqlResponse, OperationCanceledException>(_ => Observable.Empty<AppSyncGraphqlResponse>())
                .Publish();



            var maintenanceSubscription = _incomingMessages.Subscribe(
                _ => { }, 
                exception =>
                {
                    _exceptionSubject.OnNext(exception);

                    ShutdownConnection();
                },
                () =>
                {
                    ShutdownConnection();
                });





            _incomingMessages
                .Subscribe(response =>
                {
                    var responseMessage = response.Message;

                    switch (response.Type)
                    {
                        //case GraphqlResponseType.ConnectionAck:
                            //await SendRegistrationRequests();
                        //    break;

                        //case GraphqlResponseType.StartAck:
                        //    break;

                        // this is received for both data responses and response errors
                        case GraphqlResponseType.Data:
                            var subscription = GetSubscription(response.Id);
                            subscription.NotifyResponse(responseMessage);
                            break;

                        case GraphqlResponseType.KeepAlive:
                            break;

                        case GraphqlResponseType.Error: // test by providing a query rather than a subscription
                            
                            var error = JsonConvert.DeserializeObject<WebSocketResponse<GraphqlError>>(responseMessage);
                            var responseError = new GraphqlSubscriptionResponseError(response.Id, error);
                            _graphqlErrorSubject.OnNext(responseError);

                            ShutdownConnection();
                            break;

                        case GraphqlResponseType.Close:
                            // todo: close the connection and report the error ?
                            //throw new Exception("Connection closed by the server");
                            break;

                        case GraphqlResponseType.Complete:  // follows a 'stop'
                            break;
                    }
                });

            var connection = _incomingMessages.Connect();

            _incomingMessagesConnection = new CompositeDisposable(maintenanceSubscription, connection);
        }

        private AppSyncGraphqlResponse GetAppSyncGraphqlResponse(MemoryStream ms)
        {
            var response = _serializer.GetAppSyncGraphqlResponse(ms);
            response.Message = Encoding.UTF8.GetString(ms.ToArray());

            return response;
        }

        private Task SendInitRequest()
        {
            var request = new SubscriptionQueryMessage
            {
                Type = GraphqlRequestType.ConnectionInit
            };

            return SendRequest(request);
        }

        private async Task UnregisterSubscription(string id)
        {
            var request = new SubscriptionQueryMessage
            {
                Id = id,
                Type = GraphqlRequestType.Stop
            };

            var completeTask = _incomingMessages
                .TakeUntil(response => response.Id == id && response.Type == GraphqlResponseType.Complete)
                .LastAsync()
                .ToTask(CancellationToken.None);

            await SendRequest(request).ConfigureAwait(false);
            await completeTask.ConfigureAwait(false);

            _subscriptions.Remove(id);

            if (!_subscriptions.Any())
            {
                try
                {
                    _cts.Cancel();
                }
                catch (OperationCanceledException)
                {

                }

                ShutdownConnection();
            }
        }

        private async Task SendRegistrationRequests()
        {
            // make sure all registrations ACK
            using (var cts = new CancellationTokenSource())
            {
                var ackTasks = new List<Task>();

                foreach (var registration in _subscriptions.Values)
                {
                    var ackTask = await SendRegistration(registration).ConfigureAwait(false);
                    ackTasks.Add(ackTask);
                }

                var timeout = GetExpiringTask(TimeSpan.FromSeconds(5), cts);

                try
                {
                    await Task.WhenAll(ackTasks.Concat(new[] { timeout })).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // ?? something went wrong - need to deal with this
                    throw;
                }
            }
        }

        private async Task<Task<AppSyncGraphqlResponse>> SendRegistration(SubscriptionRegistration registration)
        {
            var request = registration.Request;

            var ackTask = _incomingMessages
                .TakeUntil(response => response.Id == request.Id && response.Type is GraphqlResponseType.StartAck ||
                                       response.Type == GraphqlResponseType.Error)
                .LastAsync()
                .ToTask(_cts.Token);

            await SendRequest(request).ConfigureAwait(false);

            return ackTask;
        }

        private Task SendRequest<TMessage>(TMessage message)
        {
            var buffer = _serializer.SerializeToBytes(message);
            var segment = new ArraySegment<byte>(buffer);

            return _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, _cts.Token);
        }

        private Task GetExpiringTask(TimeSpan timeSpan, CancellationTokenSource cts)
        {
            return Task
                .Delay(timeSpan)
                .ContinueWith(t => cts.Cancel(), TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }


    public interface IAppSyncClientSerializer
    {
        string SerializeObject<TType>(TType request);
        byte[] SerializeToBytes<TType>(TType request);
        TType DeserializeFromStream<TType>(Stream stream);
    }


    public static class AppSyncClientSerializerExtensions
    {
        public static AppSyncGraphqlResponse GetAppSyncGraphqlResponse(this IAppSyncClientSerializer serializer, Stream stream)
        {
            return serializer.DeserializeFromStream<AppSyncGraphqlResponse>(stream);
        }
    }


    // todo: need to create serializer packages for newtonsoft and system.text
    public sealed class AppSyncClientNewtonsoftJsonSerializer : IAppSyncClientSerializer
    {
        private readonly JsonSerializerSettings _defaultSerializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        public string SerializeObject<TMessage>(TMessage message)
        {
            return JsonConvert.SerializeObject(message, _defaultSerializerSettings);
        }

        public byte[] SerializeToBytes<TMessage>(TMessage message)
        {
            var json = SerializeObject(message);
            return Encoding.UTF8.GetBytes(json);
        }

        //public AppSyncGraphqlResponse DeserializeToAppSyncGraphqlResponse(Stream stream)
        //{
        //    return DeserializeFromStream<AppSyncGraphqlResponse>(stream);
        //}

        public TType DeserializeFromStream<TType>(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var serializer = JsonSerializer.Create(_defaultSerializerSettings);
                    var result = serializer.Deserialize<TType>(reader);
                    return result;
                }
            }
        }
    }

    //public sealed class AppSyncClientNewtonsoftJsonSerializer : IAppSyncClientSerializer
    //{
    //    public byte[] SerializeToBytes(GraphqlSubscriptionRequest request)
    //    {
    //        var json = System.Text.JsonSerializer.SerializeToUtf8Bytes(request);
    //        return Encoding.UTF8.GetBytes(json);
    //    }
    //}

}
