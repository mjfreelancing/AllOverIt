using AllOverIt.Aws.AppSync.Client.Exceptions;
using AllOverIt.Aws.AppSync.Client.Extensions;
using AllOverIt.Helpers;
using AllOverIt.Serialization.Abstractions;
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
    // Implemented as per the protocol described at:
    // https://docs.aws.amazon.com/appsync/latest/devguide/real-time-websocket-client.html

    public sealed class AppSyncSubscriptionClient
    {
        private class ExceptionCollector : IDisposable
        {
            private readonly List<Exception> _exceptions = new();
            private IDisposable _subscription;

            public IReadOnlyList<Exception> Exceptions => _exceptions;

            public ExceptionCollector(IObservable<Exception> observable)
            {
                _subscription = observable
                    .WhenNotNull(nameof(observable))
                    .Subscribe(_exceptions.Add);
            }

            public void Dispose()
            {
                _subscription?.Dispose();
                _subscription = null;
            }
        }

        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

        // todo: make this configurable
        private readonly TimeSpan _connectionTimeout = DefaultTimeout;
        private readonly TimeSpan _subscribeTimeout = DefaultTimeout;
        private readonly TimeSpan _unsubscribeTimeout = DefaultTimeout;

        private CancellationTokenSource ConnectionTimeoutSource => new(_connectionTimeout);
        private CancellationTokenSource SubscribeTimeoutSource => new(_subscribeTimeout);
        private CancellationTokenSource UnsubscribeTimeoutSource => new(_unsubscribeTimeout);

        private readonly string _host;
        private readonly Uri _uri;
        private readonly IAuthorization _defaultAuthorization;
        private readonly IJsonSerializer _serializer;
        private readonly ArraySegment<byte> _buffer = new(new byte[8192]);
        private readonly IDictionary<string, SubscriptionRegistration> _subscriptions = new ConcurrentDictionary<string, SubscriptionRegistration>();
        private CancellationTokenSource _cts;
        private IConnectableObservable<AppSyncGraphqlResponse> _incomingMessages;
        private IDisposable _incomingMessagesConnection;

        // One WebSocket connection can have multiple subscriptions (even with different authentication modes).
        private ClientWebSocket _webSocket;

        // Only allow a single subscription to be processed at a time, just in case there are communication errors during
        // the connection or registration handshaking. Trying to deal with overlapping state is too complex.
        private readonly SemaphoreSlim _subscriptionLock = new(1, 1);

        private readonly BehaviorSubject<SubscriptionConnectionState> _connectionStateSubject = new(SubscriptionConnectionState.Disconnected);
        private readonly Subject<Exception> _exceptionSubject = new();
        private readonly Subject<GraphqlSubscriptionResponseError> _graphqlErrorSubject = new();

        public IObservable<SubscriptionConnectionState> ConnectionState => _connectionStateSubject;
        public IObservable<Exception> Exceptions => _exceptionSubject;
        public IObservable<GraphqlSubscriptionResponseError> GraphqlErrors => _graphqlErrorSubject;

        // todo: consider DI implications
        // todo: create a configuration class that everything can be added to
        // endpoint is the graphql endpoint (not realtime) without https, wss, or /graphql
        // e.g., example123abc.appsync-api.ap-southeast-2.amazonaws.com
        public AppSyncSubscriptionClient(string host, IAuthorization defaultAuthorization, IJsonSerializer serializer)
            : this(host, host?.Replace("appsync-api", "appsync-realtime-api"), defaultAuthorization, serializer)
        {
        }

        public AppSyncSubscriptionClient(string host, string realtime, IAuthorization defaultAuthorization, IJsonSerializer serializer)
        {
            _host = host.WhenNotNullOrEmpty(nameof(host));
            _ = realtime.WhenNotNullOrEmpty(nameof(realtime));

            var hostAuth = new HostAuthorization(host, defaultAuthorization);
            var encodedHeader = hostAuth.GetEncodedHeader();

            _uri = new Uri($"wss://{realtime}/graphql?header={encodedHeader}&payload=e30=");

            _defaultAuthorization = defaultAuthorization.WhenNotNull(nameof(defaultAuthorization));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        // The default authorization mode will be used if authorization is null.
        public async Task<SubscriptionId> SubscribeAsync<TResponse>(SubscriptionQuery query, Action<SubscriptionResponse<TResponse>> responseAction,
            IAuthorization authorization = null)
        {
            // Only allow a single registration at a time to avoid complex overlapping connection states when there's a WebSocket issue.
            await _subscriptionLock.WaitAsync().ConfigureAwait(false);

            try
            {
                return await _connectionStateSubject.WaitUntil(
                    state => state is
                        SubscriptionConnectionState.Disconnected or
                        SubscriptionConnectionState.Connected or
                        SubscriptionConnectionState.KeepAlive,
                    async _ =>
                    {
                        using (var subscriptionErrors = GetExceptionCollector())
                        {
                            try
                            {
                                // Will connect (and wait for ACK) if currently disconnected
                                var connectionState = await CheckWebSocketConnectionAsync().ConfigureAwait(false);

                                // Abort if the connection failed
                                if (connectionState == SubscriptionConnectionState.Disconnected ||
                                    connectionState == SubscriptionConnectionState.Disconnecting)
                                {
                                    return new SubscriptionId(subscriptionErrors.Exceptions);
                                }

                                authorization ??= _defaultAuthorization;

                                var hostAuthorization = new HostAuthorization(_host, authorization);

                                var payload = new SubscriptionQueryPayload
                                {
                                    Data = _serializer.SerializeObject(query),
                                    Extensions = new {authorization = hostAuthorization.KeyValues}
                                };

                                var registration =
                                    new SubscriptionRegistration<TResponse>(query.Id, payload, responseAction,
                                        _serializer);

                                _subscriptions.Add(registration.Id, registration);

                                var response = await SendRegistrationAsync(registration).ConfigureAwait(false);

                                if (response.Type == GraphqlResponseType.Error)
                                {
                                    var graphqlErrorMessage = GetGraphqlErrorFromResponseMessage(response.Message);
                                    return new SubscriptionId(registration.Id, graphqlErrorMessage.Payload.Errors);
                                }

                                // This is decorated by SubscriptionId to avoid leaking SubscriptionRegistration
                                var disposable = new RaiiAsync<SubscriptionRegistration>(
                                    () => registration,
                                    async subscription =>
                                    {
                                        await UnregisterSubscriptionAsync(subscription.Id).ConfigureAwait(false);
                                    });

                                return new SubscriptionId(registration.Id, disposable);
                            }
                            catch (Exception exception)
                            {
                                // Not shutting down the connection here - it will have already been dealt with

                                // GraphqlConnectionException
                                // GraphqlConnectionTimeoutException
                                // GraphqlSubscribeTimeoutException
                                // GraphqlUnsubscribeTimeoutException
                                // WebSocketConnectionLostException - if the websocket is shutdown mid-subscription registration
                                _exceptionSubject.OnNext(exception);
                                return new SubscriptionId(subscriptionErrors.Exceptions);
                            }
                        }
                    });
            }
            finally
            {
                _subscriptionLock.Release();
            }
        }

        private async Task<SubscriptionConnectionState> CheckWebSocketConnectionAsync()
        {
            return await _connectionStateSubject
                .WaitUntil<SubscriptionConnectionState>(state => state is
                    SubscriptionConnectionState.Disconnected or
                    SubscriptionConnectionState.Connected or
                    SubscriptionConnectionState.KeepAlive,
                async _ =>
                {
                    if (_webSocket is { State: WebSocketState.Open })
                    {
                        return SubscriptionConnectionState.Connected;
                    }

                    // dispose if not currently in an Open state
                    DisposeCommunicationResources();

                    _cts = new CancellationTokenSource();

                    _connectionStateSubject.OnNext(SubscriptionConnectionState.Connecting);

                    _webSocket = new ClientWebSocket();
                    _webSocket.Options.AddSubProtocol("graphql-ws");

                    await _webSocket.ConnectAsync(_uri, _cts.Token).ConfigureAwait(false);

                    // This method may receive a "connection_error" and handle a WebSocketException exception
                    // before this method returns - the exception will be notified via the Exceptions observable.
                    ConfigureMessageProcessing();

                    // If a websocket exception occurred above then we cannot continue
                    if (_connectionStateSubject.Value != SubscriptionConnectionState.Disconnected)
                    {
                        using (var timeoutSource = ConnectionTimeoutSource)
                        {
                            try
                            {
                                using (var linkedCts = GetConnectionLinkedTokenSource(timeoutSource.Token))
                                {
                                    var ack = _incomingMessages
                                        .TakeUntil(response => response is
                                        {
                                            Type: GraphqlResponseType.ConnectionAck or GraphqlResponseType
                                                .ConnectionError
                                        })
                                        .LastAsync()
                                        .ToTask(linkedCts.Token);

                                    await SendInitRequestAsync().ConfigureAwait(false);

                                    // if there's an error, the process will be aborted via a cancellation
                                    var response = await ack.ConfigureAwait(false);

                                    if (response.Type == GraphqlResponseType.ConnectionAck)
                                    {
                                        _connectionStateSubject.OnNext(SubscriptionConnectionState.Connected);

                                        // re-register any existing subscriptions
                                        await SendRegistrationRequestsAsync().ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        var error = GetGraphqlErrorFromResponseMessage(response.Message);
                                        throw new GraphqlConnectionException(error);
                                    }
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                // if _cts was cancelled then there was a connection issue - the exception will have been captured
                                // from the _exceptionSubject (being observed at the start of the subscription process)
                                if (timeoutSource.Token.IsCancellationRequested)
                                {
                                    throw new GraphqlConnectionTimeoutException(_connectionTimeout);
                                }
                            }
                        }
                    }

                    return _connectionStateSubject.Value;
                },
                ex =>
                {
                    ShutdownConnection();
                });
        }

        private void ShutdownConnection()
        {
            var isShuttingDown = _connectionStateSubject.Value is
                SubscriptionConnectionState.Disconnecting or
                SubscriptionConnectionState.Disconnected;

            // Only disconnect if currently Connecting, Connected, or KeepAlive were the last known states
            if (!isShuttingDown)
            {
                _connectionStateSubject.OnNext(SubscriptionConnectionState.Disconnecting);

                DisposeCommunicationResources();

                _connectionStateSubject.OnNext(SubscriptionConnectionState.Disconnected);
            }
        }

        private void DisposeCommunicationResources()
        {
            try
            {
                _cts?.Cancel();
            }
            catch (OperationCanceledException)
            {
                // should never fail, but here just in case
            }

            // disposes of all subscriptions
            _incomingMessagesConnection?.Dispose();
            _incomingMessagesConnection = null;

            // attempting to close the connection fails if the internal stream has been closed due to an error - disposing it is fine
            _webSocket?.Dispose();
            _webSocket = null;

            _cts?.Dispose();
            _cts = null;
        }

        private SubscriptionRegistration GetSubscription(string id)
        {
            if (_subscriptions.TryGetValue(id, out var subscription))
            {
                return subscription;
            }

            throw new KeyNotFoundException($"Subscription Id '{id}' not found.");
        }

        private async Task<AppSyncGraphqlResponse> GetIncomingMessageAsync()
        {
            using (var stream = new MemoryStream())
            {
                WebSocketReceiveResult webSocketReceiveResult;

                do
                {
                    // WebSocketException is reported via shutdownSubscription
                    webSocketReceiveResult = await _webSocket.ReceiveAsync(_buffer, _cts.Token).ConfigureAwait(false);

                    if (!_cts.Token.IsCancellationRequested)
                    {
                        stream.Write(_buffer.Array!, _buffer.Offset, webSocketReceiveResult.Count);
                    }
                } while (!webSocketReceiveResult.EndOfMessage && !_cts.Token.IsCancellationRequested);

                _cts.Token.ThrowIfCancellationRequested();

                stream.Seek(0, SeekOrigin.Begin);

                switch (webSocketReceiveResult.MessageType)
                {
                    case WebSocketMessageType.Text:
                    case WebSocketMessageType.Close:
                        return GetAppSyncGraphqlResponse(stream);

                    default:
                        throw new InvalidOperationException($"Unexpected websocket message type '{webSocketReceiveResult.MessageType}'.");
                }
            }
        }

        private void ConfigureMessageProcessing()
        {
            // Subscribe to the incoming messages independent of the exception handling so the message
            // processing (_incomingMessages) can be disposed before the socket connection is disposed.
            _incomingMessages = Observable
                .Defer(() => Observable.FromAsync(GetIncomingMessageAsync))
                .Repeat()
                .Catch<AppSyncGraphqlResponse, OperationCanceledException>(_ => Observable.Empty<AppSyncGraphqlResponse>())
                .Publish();

            // Process exceptions by closing the WebSocket connection - not auto-reconnecting in case the
            // issue results in a permanent loop that cannot be escaped.
            var shutdownSubscription = _incomingMessages
                .Subscribe(
                    _ => { },
                    exception =>
                    {
                        _exceptionSubject.OnNext(exception);
                        ShutdownConnection();
                    },
                    () => { });

            // Process messages related to data responses/errors, keep alive notifications, and server-side termination.
            var messageSubscription = _incomingMessages
                .Where(item => item.Type is
                    GraphqlResponseType.ConnectionError or
                    GraphqlResponseType.Data or
                    GraphqlResponseType.KeepAlive or
                    GraphqlResponseType.Error or
                    GraphqlResponseType.Close)
                .Subscribe(response =>
                {
                    var responseMessage = response.Message;

                    switch (response.Type)
                    {
                        // Such as a websocket issue - this can be received here or within CheckWebSocketConnectionAsync(), timing dependent
                        case GraphqlResponseType.ConnectionError:   // response.Id will be null
                            NotifySubscriptionError(null, responseMessage);
                            break;

                        // Applies to both data responses and response errors (such as mapping issues)
                        case GraphqlResponseType.Data:
                            var subscription = GetSubscription(response.Id);
                            subscription.NotifyResponse(responseMessage);
                            break;

                        case GraphqlResponseType.KeepAlive:         // todo: ? track and kill / restore the connection and re-subscribe
                            _connectionStateSubject.OnNext(SubscriptionConnectionState.KeepAlive);
                            break;

                        // An error specific to a specific subscription (such as a malformed query).
                        case GraphqlResponseType.Error:
                            NotifySubscriptionError(response.Id, responseMessage);
                            break;

                        // AppSync has closed the connection
                        case GraphqlResponseType.Close:
                            NotifySubscriptionError(response.Id, responseMessage);
                            ShutdownConnection();
                            break;
                    }
                });

            try
            {
                // connect all subscriptions to the source
                var connection = _incomingMessages.Connect();

                _incomingMessagesConnection = new CompositeDisposable(messageSubscription, shutdownSubscription, connection);
            }
            catch (WebSocketException)
            {
                // This error has been seen when the websocket sub-protocol has not been set.
                // The error would have been reported via the Exceptions observable - avoid raising it again
            }
        }

        private void NotifySubscriptionError(string id, string responseMessage)
        {
            var error = GetGraphqlErrorFromResponseMessage(responseMessage);
            var responseError = new GraphqlSubscriptionResponseError(id, error);
            _graphqlErrorSubject.OnNext(responseError);
        }

        private Task SendInitRequestAsync()
        {
            var request = new SubscriptionQueryMessage
            {
                Type = GraphqlRequestType.ConnectionInit
            };

            return SendRequest(request);
        }

        private async Task UnregisterSubscriptionAsync(string id)
        {
            try
            {
                using (var timeoutSource = UnsubscribeTimeoutSource)
                {
                    var request = new SubscriptionQueryMessage
                    {
                        Id = id,
                        Type = GraphqlRequestType.Stop
                    };

                    using (var linkedCts = GetConnectionLinkedTokenSource(timeoutSource.Token))
                    {
                        var completeTask = _incomingMessages
                            .TakeUntil(response => response.Id == id && response.Type == GraphqlResponseType.Complete)
                            .LastAsync()
                            .ToTask(linkedCts.Token);

                        await SendRequest(request).ConfigureAwait(false);
                        await completeTask.ConfigureAwait(false);
                    }
                }
            }
            catch (WebSocketConnectionLostException)
            {
                // The connection was lost after the message was sent.
                // The error would have been reported, do nothing here.
            }
            catch (OperationCanceledException)
            {
                // We can't throw from here - the subscription is being disposed of within a RaiiAsync instance.
                // The exception will not be observed.
                var timeoutException = new GraphqlUnsubscribeTimeoutException(id, _unsubscribeTimeout);
                _exceptionSubject.OnNext(timeoutException);
            }
            finally
            {
                // needs to be removed because the subscription is being disposed
                _subscriptions.Remove(id);

                // shutdown the web socket if there's no more registered subscriptions
                if (!_subscriptions.Any())
                {
                    ShutdownConnection();
                }
            }
        }

        private async Task SendRegistrationRequestsAsync()
        {
            // todo: throw if not completed within a give time period
            // make sure all registrations ACK
            var ackTasks = new List<Task>();

            foreach (var registration in _subscriptions.Values)
            {
                // todo: deal with subscription timeouts, loss of connectivity
                var ackTask = SendRegistrationAsync(registration);
                ackTasks.Add(ackTask);
            }

            await Task.WhenAll(ackTasks).ConfigureAwait(false);
        }

        private async Task<AppSyncGraphqlResponse> SendRegistrationAsync(SubscriptionRegistration registration)
        {
            try
            {
                using (var timeoutSource = SubscribeTimeoutSource)
                {
                    using (var linkedCts = GetConnectionLinkedTokenSource(timeoutSource.Token))
                    {
                        var request = registration.Request;

                        var ackTask = _incomingMessages
                            .TakeUntil(response =>
                                response.Id == request.Id &&
                                response.Type is GraphqlResponseType.StartAck or GraphqlResponseType.Error)
                            .LastAsync()
                            .ToTask(linkedCts.Token);

                        await SendRequest(request).ConfigureAwait(false);
                        return await ackTask.ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // If _cts was cancelled then there was a connection issue - probably wouldn't even get here unless it
                // was the timeout token that cancelled. In reality, if _cts is cancelled then the connection was most
                // likely shutdown and an exception would have been thrown - not caught in this method.
                if (UnsubscribeTimeoutSource.IsCancellationRequested)
                {
                    throw new GraphqlSubscribeTimeoutException(registration.Id, _unsubscribeTimeout);
                }

                // Probably won't get here but is needed to keep the compiler happy. Worse case, the exception is reported twice.
                throw;
            }
        }

        private Task SendRequest<TMessage>(TMessage message)
        {
            var buffer = _serializer.SerializeToBytes(message);
            var segment = new ArraySegment<byte>(buffer);

            // check if an error has occurred mid-subscription that resulted in the WebSocket being disposed
            if (_webSocket == null)
            {
                throw new WebSocketConnectionLostException();
            }

            return _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, _cts.Token);
        }

        private WebSocketResponse<GraphqlError> GetGraphqlErrorFromResponseMessage(string message)
        {
            return _serializer.DeserializeObject<WebSocketResponse<GraphqlError>>(message);
        }

        private AppSyncGraphqlResponse GetAppSyncGraphqlResponse(MemoryStream stream)
        {
            var response = _serializer.DeserializeObject<AppSyncGraphqlResponse>(stream);
            response.Message = Encoding.UTF8.GetString(stream.ToArray());

            return response;
        }

        private ExceptionCollector GetExceptionCollector()
        {
            return new ExceptionCollector(_exceptionSubject);
        }

        private CancellationTokenSource GetConnectionLinkedTokenSource(CancellationToken token)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, token);
        }
    }
}
