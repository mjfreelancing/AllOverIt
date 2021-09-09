using AllOverIt.Aws.AppSync.Client.Exceptions;
using AllOverIt.Aws.AppSync.Client.Extensions;
using AllOverIt.Aws.AppSync.Client.Subscription.Configuration;
using AllOverIt.Aws.AppSync.Client.Utils;
using AllOverIt.Helpers;
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

// todo:
// - Cognito
// - internal connection recovery
// - client-side connect/disconnect without loss of subscriptions

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class TimeoutCancellationSource : IDisposable
    {
        private CancellationTokenSource _cts;

        public CancellationToken Token => _cts.Token;
        public TimeSpan Timeout { get; }

        public TimeoutCancellationSource(TimeSpan timeout)
        {
            Timeout = timeout;
            _cts = new CancellationTokenSource(timeout);
        }

        public void Dispose()
        {
            _cts?.Dispose();
            _cts = null;
        }

        public CancellationTokenSource GetLinkedTokenSource(CancellationToken token)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(Token, token);
        }
    }


    // Implemented as per the protocol described at:
    // https://docs.aws.amazon.com/appsync/latest/devguide/real-time-websocket-client.html

    public sealed class AppSyncSubscriptionClient
    {
        //private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

        // todo: make this configurable
        //private readonly TimeSpan _connectionTimeout = DefaultTimeout;
        //private readonly TimeSpan _subscribeTimeout = DefaultTimeout;
        //private readonly TimeSpan _unsubscribeTimeout = DefaultTimeout;
        //private CancellationTokenSource ConnectionTimeoutSource => new(_connectionTimeout);
        //private CancellationTokenSource SubscribeTimeoutSource => new(_subscribeTimeout);
        //private CancellationTokenSource UnsubscribeTimeoutSource => new(_unsubscribeTimeout);

        private readonly AppSyncSubscriptionConfiguration _configuration;
        private readonly ArraySegment<byte> _buffer = new(new byte[8192]);
        private readonly IDictionary<string, SubscriptionRegistration> _subscriptions = new ConcurrentDictionary<string, SubscriptionRegistration>();

        // the primary CancellationTokenSource used for message retrieval from the web socket
        private CancellationTokenSource _webSocketCancellationTokenSource;

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

        public AppSyncSubscriptionClient(AppSyncSubscriptionConfiguration configuration)
        {
            _configuration = configuration.WhenNotNull(nameof(configuration));
            _ = configuration.RealTimeUrl.WhenNotNullOrEmpty(nameof(configuration.RealTimeUrl));
        }

        // The default authorization mode will be used if authorization is null.
        public async Task<IAppSubscriptionRegistration> SubscribeAsync<TResponse>(SubscriptionQuery query,
            Action<SubscriptionResponse<TResponse>> responseAction, IAppSyncAuthorization authorization = null)
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
                        using (var subscriptionErrors = new ExceptionCollector(_exceptionSubject))
                        {
                            try
                            {
                                // Will connect (and wait for ACK) if currently disconnected
                                var connectionState = await CheckWebSocketConnectionAsync().ConfigureAwait(false);

                                // Abort if the connection failed
                                if (connectionState is SubscriptionConnectionState.Disconnected or SubscriptionConnectionState.Disconnecting)
                                {
                                    return new AppSubscriptionRegistration(query.Id, subscriptionErrors.Exceptions);
                                }

                                authorization ??= _configuration.DefaultAuthorization;

                                var hostAuthorization = new AppSyncHostAuthorization(_configuration.HostUrl, authorization);

                                var payload = new SubscriptionQueryPayload
                                {
                                    Data = _configuration.Serializer.SerializeObject(query),
                                    Extensions = new {authorization = hostAuthorization.KeyValues}
                                };

                                var registration = new SubscriptionRegistration<TResponse>(query.Id, payload, responseAction, _configuration.Serializer);

                                // registration.Id will be the same as query.Id
                                _subscriptions.Add(query.Id, registration);

                                var response = await SendRegistrationAsync(registration).ConfigureAwait(false);

                                if (response.Type == GraphqlResponseType.Error)
                                {
                                    var graphqlErrorMessage = GetGraphqlErrorFromResponseMessage(response.Message);
                                    return new AppSubscriptionRegistration(query.Id, graphqlErrorMessage.Payload.Errors);
                                }

                                // This is decorated by AppSubscriptionRegistration to avoid leaking SubscriptionRegistration
                                var disposable = new RaiiAsync<SubscriptionRegistration>(
                                    () => registration,
                                    async subscription =>
                                    {
                                        await UnregisterSubscriptionAsync(subscription.Id).ConfigureAwait(false);
                                    });

                                return new AppSubscriptionRegistration(query.Id, disposable);
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
                                return new AppSubscriptionRegistration(query.Id, subscriptionErrors.Exceptions);
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
            if (_connectionStateSubject.Value is SubscriptionConnectionState.Connected or SubscriptionConnectionState.KeepAlive)
            {
                return SubscriptionConnectionState.Connected;
            }

            return await _connectionStateSubject
                .WaitUntil<SubscriptionConnectionState>(state => state == SubscriptionConnectionState.Disconnected,
                async _ =>
                {
                    _connectionStateSubject.OnNext(SubscriptionConnectionState.Connecting);

                    await ConnectWebSocketAsync();

                    // This method may receive a "connection_error" and handle a WebSocketException exception
                    // before this method returns - the exception will be notified via the Exceptions observable.
                    ConfigureMessageProcessing();

                    // If a websocket exception occurred above then we cannot continue
                    if (_connectionStateSubject.Value != SubscriptionConnectionState.Disconnected)
                    {
                        using (var timeoutSource = new TimeoutCancellationSource(_configuration.ConnectionOptions.ConnectionTimeout))
                        {
                            try
                            {
                                using (var linkedCts = timeoutSource.GetLinkedTokenSource(_webSocketCancellationTokenSource.Token))
                                {
                                    var ack = _incomingMessages
                                        .TakeUntil(response => response is
                                        {
                                            Type: GraphqlResponseType.ConnectionAck or GraphqlResponseType.ConnectionError
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
                                // if _webSocketCancellationTokenSource was cancelled then there was a connection issue - the exception will have been captured
                                // from the _exceptionSubject (being observed at the start of the subscription process)
                                if (timeoutSource.Token.IsCancellationRequested)
                                {
                                    throw new GraphqlConnectionTimeoutException(timeoutSource.Timeout);
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

        private Task ConnectWebSocketAsync()
        {
            _webSocketCancellationTokenSource = new CancellationTokenSource();

            var hostAuth = new AppSyncHostAuthorization(_configuration.HostUrl, _configuration.DefaultAuthorization);
            var encodedHeader = hostAuth.GetEncodedHeader();
            var uri = new Uri($"wss://{_configuration.RealTimeUrl}/graphql?header={encodedHeader}&payload=e30=");

            _webSocket = new ClientWebSocket();
            _webSocket.Options.AddSubProtocol("graphql-ws");

            return _webSocket.ConnectAsync(uri, _webSocketCancellationTokenSource.Token);
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
                _webSocketCancellationTokenSource?.Cancel();
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

            _webSocketCancellationTokenSource?.Dispose();
            _webSocketCancellationTokenSource = null;
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
                    webSocketReceiveResult = await _webSocket.ReceiveAsync(_buffer, _webSocketCancellationTokenSource.Token).ConfigureAwait(false);

                    if (!_webSocketCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        stream.Write(_buffer.Array!, _buffer.Offset, webSocketReceiveResult.Count);
                    }
                } while (!webSocketReceiveResult.EndOfMessage && !_webSocketCancellationTokenSource.Token.IsCancellationRequested);

                _webSocketCancellationTokenSource.Token.ThrowIfCancellationRequested();

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
            using (var timeoutSource = new TimeoutCancellationSource(_configuration.ConnectionOptions.SubscriptionTimeout))
            {
                try
                {
                    var request = new SubscriptionQueryMessage
                    {
                        Id = id,
                        Type = GraphqlRequestType.Stop
                    };

                    using (var linkedCts = timeoutSource.GetLinkedTokenSource(_webSocketCancellationTokenSource.Token))
                    {
                        var completeTask = _incomingMessages
                            .TakeUntil(response => response.Id == id && response.Type == GraphqlResponseType.Complete)
                            .LastAsync()
                            .ToTask(linkedCts.Token);

                        await SendRequest(request).ConfigureAwait(false);
                        await completeTask.ConfigureAwait(false);
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
                    var timeoutException = new GraphqlUnsubscribeTimeoutException(id, timeoutSource.Timeout);
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
            using (var timeoutSource = new TimeoutCancellationSource(_configuration.ConnectionOptions.SubscriptionTimeout))
            {
                try
                {
                    using (var linkedCts = timeoutSource.GetLinkedTokenSource(_webSocketCancellationTokenSource.Token))
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
                catch (OperationCanceledException)
                {
                    // If _webSocketCancellationTokenSource was cancelled then there was a connection issue - probably wouldn't even get here unless it
                    // was the timeout token that cancelled. In reality, if _webSocketCancellationTokenSource is cancelled then the connection was most
                    // likely shutdown and an exception would have been thrown - not caught in this method.
                    if (timeoutSource.Token.IsCancellationRequested)
                    {
                        throw new GraphqlSubscribeTimeoutException(registration.Id, timeoutSource.Timeout);
                    }

                    // Probably won't get here but is needed to keep the compiler happy. Worse case, the exception is reported twice.
                    throw;
                }
            }
        }

        private Task SendRequest<TMessage>(TMessage message)
        {
            var buffer = _configuration.Serializer.SerializeToBytes(message);
            var segment = new ArraySegment<byte>(buffer);

            // check if an error has occurred mid-subscription that resulted in the WebSocket being disposed
            if (_webSocket == null)
            {
                throw new WebSocketConnectionLostException();
            }

            return _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, _webSocketCancellationTokenSource.Token);
        }

        private WebSocketGraphqlResponse<GraphqlError> GetGraphqlErrorFromResponseMessage(string message)
        {
            return _configuration.Serializer.DeserializeObject<WebSocketGraphqlResponse<GraphqlError>>(message);
        }

        private AppSyncGraphqlResponse GetAppSyncGraphqlResponse(MemoryStream stream)
        {
            var response = _configuration.Serializer.DeserializeObject<AppSyncGraphqlResponse>(stream);
            response.Message = Encoding.UTF8.GetString(stream.ToArray());

            return response;
        }
    }
}
