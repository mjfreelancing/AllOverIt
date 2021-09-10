using AllOverIt.Aws.AppSync.Client.Exceptions;
using AllOverIt.Aws.AppSync.Client.Extensions;
using AllOverIt.Aws.AppSync.Client.Subscription.Authorization;
using AllOverIt.Aws.AppSync.Client.Subscription.Configuration;
using AllOverIt.Aws.AppSync.Client.Subscription.Registration;
using AllOverIt.Aws.AppSync.Client.Subscription.Request;
using AllOverIt.Aws.AppSync.Client.Subscription.Response;
using AllOverIt.Aws.AppSync.Client.Utils;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    /// <summary>An AppSync subscription client that supports API KEY and Cognito based authorization.</summary>
    /// <remarks>Implemented as per the protocol described at https://docs.aws.amazon.com/appsync/latest/devguide/real-time-websocket-client.html.</remarks>
    public sealed class AppSyncSubscriptionClient
    {
        private readonly AppSyncSubscriptionConfiguration _configuration;
        private readonly ArraySegment<byte> _buffer = new(new byte[8192]);
        private readonly IDictionary<string, SubscriptionRegistrationRequest> _subscriptions = new ConcurrentDictionary<string, SubscriptionRegistrationRequest>();

        // Only allow a single subscription to be processed at a time, just in case there are communication errors during
        // the connection or registration handshaking. Trying to deal with overlapping state is too complex.
        private readonly SemaphoreSlim _subscriptionLock = new(1, 1);

        private readonly BehaviorSubject<SubscriptionConnectionState> _connectionStateSubject = new(SubscriptionConnectionState.Disconnected);
        private readonly Subject<Exception> _exceptionSubject = new();
        private readonly Subject<GraphqlSubscriptionResponseError> _graphqlErrorSubject = new();

        private TimeSpan _healthCheckPeriod;        // Max period to wait before a keep alive message from AppSync
        private IDisposable _healthDisposable;      // Subscription for resetting the connection and re-subscribing all subscriptions
        private IConnectableObservable<AppSyncGraphqlResponse> _incomingMessages;
        private IDisposable _incomingMessagesConnection;

        // the primary CancellationTokenSource used for message retrieval from the web socket
        private CancellationTokenSource _webSocketCancellationTokenSource;

        // One WebSocket connection can have multiple subscriptions (even with different authentication modes).
        private ClientWebSocket _webSocket;

        // The last raised connection state
        private SubscriptionConnectionState CurrentConnectionState => _connectionStateSubject.Value;

        public IObservable<SubscriptionConnectionState> ConnectionState => _connectionStateSubject;
        public IObservable<Exception> Exceptions => _exceptionSubject;
        public IObservable<GraphqlSubscriptionResponseError> GraphqlErrors => _graphqlErrorSubject;

        public bool IsAlive => CurrentConnectionState is SubscriptionConnectionState.Connected or SubscriptionConnectionState.KeepAlive;

        public AppSyncSubscriptionClient(AppSyncSubscriptionConfiguration configuration)
        {
            _configuration = configuration.WhenNotNull(nameof(configuration));
            _ = configuration.RealTimeUrl.WhenNotNullOrEmpty(nameof(configuration.RealTimeUrl));
        }

        /// <summary>Opens a WebSocket connection and registers the client with AppSync. Any existing subscriptions will
        /// also be re-subscribed.</summary>
        /// <returns>True if the connection was established, otherwise false.</returns>
        /// <remarks>Refer to <see cref="DisconnectAsync"/> for more information on how existing subscriptions are retained
        /// if the client is disconnected while there are active subscriptions.</remarks>
        public async Task<bool> ConnectAsync()
        {
            await CheckWebSocketConnectionAsync();
            return IsAlive;
        }

        /// <summary>Unsubscribes any existing subscriptions and then disconnects the client from AppSync. The subscription
        /// registrations are maintained (not disposed of). If a new connection is later established by calling
        /// <see cref="ConnectAsync"/> or making a new subscription then all previous subscriptions will be re-subscribed.</summary>
        /// <exception cref="UnsubscribeTimeoutException"></exception>
        public async Task DisconnectAsync()
        {
            var aggregator = new ExceptionAggregator();

            try
            {
                foreach (var subscriptionId in _subscriptions.Keys)
                {
                    try
                    {
                        // unsubscribe from AppSync but do not remove them from the registry
                        await UnsubscribeSubscriptionAsync(subscriptionId, false);
                    }
                    catch (Exception exception)
                    {
                        // UnsubscribeTimeoutException
                        aggregator.AddException(exception);
                    }
                }

                // If there was an exception above then the connection should already be closed
                ShutdownConnection();
            }
            catch (Exception exception)
            {
                aggregator.AddException(exception);
            }
            finally
            {
                aggregator.ThrowIfAnyExceptions();
            }
        }

        // The default authorization mode will be used if authorization is null.
        public async Task<IAppSyncSubscriptionRegistration> SubscribeAsync<TResponse>(SubscriptionQuery query,
            Action<SubscriptionResponse<TResponse>> responseAction, IAppSyncAuthorization authorization = null)
        {
            // Only allow a single registration at a time to avoid complex overlapping connection states when there's a WebSocket issue.
            await _subscriptionLock.WaitAsync().ConfigureAwait(false);

            try
            {
                return await _connectionStateSubject.WaitUntilAsync(
                    state => state is
                        SubscriptionConnectionState.Disconnected or
                        SubscriptionConnectionState.Connected or
                        SubscriptionConnectionState.KeepAlive,
                    async _ =>
                    {
                        using (var subscriptionErrors = new ObservableExceptionCollector(_exceptionSubject))
                        {
                            try
                            {
                                // Will connect (and wait for ACK) if currently disconnected
                                var connectionState = await CheckWebSocketConnectionAsync().ConfigureAwait(false);

                                // Abort if the connection failed
                                if (connectionState is SubscriptionConnectionState.Disconnected or SubscriptionConnectionState.Disconnecting)
                                {
                                    return new AppSyncSubscriptionRegistration(query.Id, subscriptionErrors.Exceptions);
                                }

                                authorization ??= _configuration.DefaultAuthorization;

                                var hostAuthorization = new AppSyncHostAuthorization(_configuration.HostUrl, authorization);

                                var payload = new SubscriptionQueryPayload
                                {
                                    Data = _configuration.Serializer.SerializeObject(query),
                                    Extensions = new {authorization = hostAuthorization.KeyValues}
                                };

                                var registration = new SubscriptionRegistrationRequest<TResponse>(query.Id, payload, responseAction, _configuration.Serializer);

                                // registration.Id will be the same as query.Id
                                _subscriptions.Add(query.Id, registration);

                                var response = await SendRegistrationAsync(registration).ConfigureAwait(false);

                                if (response.Type == ProtocolMessage.Response.Error)
                                {
                                    var graphqlErrorMessage = response.GetGraphqlErrorFromResponseMessage(_configuration.Serializer);
                                    return new AppSyncSubscriptionRegistration(query.Id, graphqlErrorMessage.Payload.Errors);
                                }

                                // The disposable unsubscribes from AppSync
                                var disposable = CreateSubscriptionDisposable(registration);

                                return new AppSyncSubscriptionRegistration(query.Id, disposable);
                            }
                            catch (Exception exception)
                            {
                                // Not shutting down the connection here - it will have already been dealt with

                                // ConnectionException
                                // ConnectionTimeoutException
                                // SubscribeTimeoutException
                                // UnsubscribeTimeoutException
                                // ConnectionLostException - if the websocket is shutdown mid-subscription registration
                                _exceptionSubject.OnNext(exception);
                                return new AppSyncSubscriptionRegistration(query.Id, subscriptionErrors.Exceptions);
                            }
                        }
                    });
            }
            finally
            {
                _subscriptionLock.Release();
            }
        }

        private IAsyncDisposable CreateSubscriptionDisposable(SubscriptionRegistrationRequest registration)
        {
            return new RaiiAsync<SubscriptionRegistrationRequest>(
                () => registration,
                async subscription =>
                {
                    // this is a disposal, so don't allow any uncaught exceptions
                    try
                    {
                        await UnsubscribeSubscriptionAsync(subscription.Id, true).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        _exceptionSubject.OnNext(exception);
                    }
                });
        }

        private async Task<SubscriptionConnectionState> CheckWebSocketConnectionAsync()
        {
            if (IsAlive)
            {
                return SubscriptionConnectionState.Connected;
            }

            return await _connectionStateSubject
                .WaitUntilAsync<SubscriptionConnectionState>(state => state == SubscriptionConnectionState.Disconnected,
                async _ =>
                {
                    _connectionStateSubject.OnNext(SubscriptionConnectionState.Connecting);

                    await ConnectWebSocketAsync();

                    // This method may receive a "connection_error" and handle a WebSocketException exception
                    // before this method returns - the exception will be notified via the Exceptions observable.
                    ConfigureMessageProcessing();

                    // If a websocket exception occurred above then we cannot continue
                    if (CurrentConnectionState != SubscriptionConnectionState.Disconnected)
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
                                            Type: ProtocolMessage.Response.ConnectionAck or ProtocolMessage.Response.ConnectionError
                                        })
                                        .LastAsync()
                                        .ToTask(linkedCts.Token);

                                    await SendConnectionInitRequestAsync().ConfigureAwait(false);

                                    // if there's an error, the process will be aborted via a cancellation
                                    var response = await ack.ConfigureAwait(false);

                                    // The main message processing subscription takes care of creating the health check subscription
                                    // when GraphqlResponseType.ConnectionAck is received (because it also refreshed it periodically)
                                    if (response.Type == ProtocolMessage.Response.ConnectionAck)
                                    {
                                        _connectionStateSubject.OnNext(SubscriptionConnectionState.Connected);

                                        // re-register any existing subscriptions
                                        await SendRegistrationRequestsAsync().ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        var error = response.GetGraphqlErrorFromResponseMessage(_configuration.Serializer);
                                        throw new ConnectionException(error);
                                    }
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                // if _webSocketCancellationTokenSource was cancelled then there was a connection issue - the exception
                                // will have been captured from the _exceptionSubject (being observed at the start of the subscription process)
                                if (timeoutSource.Token.IsCancellationRequested)
                                {
                                    throw new ConnectionTimeoutException(timeoutSource.Timeout);
                                }
                            }
                        }
                    }

                    // If there was a connection issue, this could be Disconnecting or Disconnected, depending on whether the
                    // error was immediately raised or sometime later during the incoming message processing.
                    return CurrentConnectionState;
                },
                ex =>
                {
                    ShutdownConnection();
                });
        }

        private void ResetHealthCheck(AppSyncGraphqlResponse response = default)
        {
            if (response != null)
            {
                var data = response.GetConnectionResponseData(_configuration.Serializer);
                _healthCheckPeriod = TimeSpan.FromMilliseconds(data.ConnectionTimeoutMs);
            }

            _healthDisposable?.Dispose();

            _healthDisposable = Observable
                .Timer(_healthCheckPeriod)
                .SelectMany(async _ =>
                {
                    _connectionStateSubject.OnNext(SubscriptionConnectionState.ConnectionReset);

                    // No point trying to disconnect via DisconnectAsync() as we're responding to the lack of communication
                    ShutdownConnection();

                    // Re-connects and re-subscribes all subscriptions
                    await ConnectAsync();

                    if (CurrentConnectionState == SubscriptionConnectionState.Disconnected)
                    {
                        // If we don't have a connection then it's time to bail - the client will need to retry
                        // as once this handler exits we have no way to re-try.
                        var connectionLost = new ConnectionLostException();
                        _exceptionSubject.OnNext(connectionLost);
                    }

                    return Unit.Default;
                })
                .Subscribe();
        }

        private Task ConnectWebSocketAsync()
        {
            _webSocketCancellationTokenSource = new CancellationTokenSource();

            var hostAuth = new AppSyncHostAuthorization(_configuration.HostUrl, _configuration.DefaultAuthorization);

            var headerValues = string.Join(",", hostAuth.KeyValues.Select(kvp => $@"""{kvp.Key}"":""{kvp.Value}"""));
            var encodedHeader = $@"{{{headerValues}}}".ToBase64();

            var uri = new Uri($"wss://{_configuration.RealTimeUrl}/graphql?header={encodedHeader}&payload=e30=");

            _webSocket = new ClientWebSocket();
            _webSocket.Options.AddSubProtocol("graphql-ws");

            return _webSocket.ConnectAsync(uri, _webSocketCancellationTokenSource.Token);
        }

        private void ShutdownConnection()
        {
            var isShuttingDown = CurrentConnectionState is
                SubscriptionConnectionState.Disconnecting or
                SubscriptionConnectionState.Disconnected;

            // Only disconnect if currently 'Connecting', 'Connected', or 'KeepAlive' were the last known states.
            // Don't use 'IsAlive' here as that does not consider 'Connecting'.
            if (!isShuttingDown)
            {
                _connectionStateSubject.OnNext(SubscriptionConnectionState.Disconnecting);

                DisposeCommunicationResources();

                _connectionStateSubject.OnNext(SubscriptionConnectionState.Disconnected);
            }
        }

        private void DisposeCommunicationResources()
        {
            // prevent the health check from kicking in
            _healthDisposable?.Dispose();
            _healthDisposable = null;

            try
            {
                _webSocketCancellationTokenSource?.Cancel();
            }
            catch (OperationCanceledException)
            {
                // should never fail, but here just in case
            }

            // stop processing web socket messages
            _incomingMessagesConnection?.Dispose();
            _incomingMessagesConnection = null;

            // attempting to close the connection fails if the internal stream has been closed due to an error - disposing it is fine
            _webSocket?.Dispose();
            _webSocket = null;

            _webSocketCancellationTokenSource?.Dispose();
            _webSocketCancellationTokenSource = null;
        }

        private SubscriptionRegistrationRequest GetSubscription(string id)
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
                        return await GetGraphqlResponse(stream);

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
                .FromAsync(GetIncomingMessageAsync)
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
                .Subscribe(response =>
                {
                    var responseMessage = response.Message;

                    switch (response.Type)
                    {
                        case ProtocolMessage.Response.ConnectionAck:
                            ResetHealthCheck(response);
                            break;

                        // Such as a websocket issue - this can be received here or within CheckWebSocketConnectionAsync(), timing dependent
                        case ProtocolMessage.Response.ConnectionError:   // response.Id will be null
                            NotifySubscriptionError(null, response);
                            break;

                        // Applies to both data responses and response errors (such as mapping issues)
                        case ProtocolMessage.Response.Data:
                            var subscription = GetSubscription(response.Id);
                            subscription.NotifyResponse(responseMessage);
                            break;

                        case ProtocolMessage.Response.KeepAlive:
                            ResetHealthCheck();
                            _connectionStateSubject.OnNext(SubscriptionConnectionState.KeepAlive);
                            break;

                        // An error specific to a specific subscription (such as a malformed query).
                        case ProtocolMessage.Response.Error:
                            NotifySubscriptionError(response.Id, response);
                            break;

                        // AppSync has closed the connection
                        case ProtocolMessage.Response.Close:
                            NotifySubscriptionError(response.Id, response);
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

        private void NotifySubscriptionError(string id, AppSyncGraphqlResponse response)
        {
            var error = response.GetGraphqlErrorFromResponseMessage(_configuration.Serializer);
            var responseError = new GraphqlSubscriptionResponseError(id, error);
            _graphqlErrorSubject.OnNext(responseError);
        }

        private Task SendConnectionInitRequestAsync()
        {
            var request = new SubscriptionQueryMessage
            {
                Type = ProtocolMessage.Request.ConnectionInit
            };

            return SendRequestAsync(request);
        }

        private async Task UnsubscribeSubscriptionAsync(string id, bool removeFromRegistry)
        {
            // It's possible to explicitly disconnect without unsubscribing a subscription (it will be re-subscribed
            // later when re-opening the connection).
            if (!IsAlive)
            {
                // this implies the subscription is being disposed of after the connection was closed (by the consumer)
                // so just remove it from the collection of subscriptions.
                if(removeFromRegistry)
                {
                    _subscriptions.Remove(id);
                }

                return;
            }

            using (var timeoutSource = new TimeoutCancellationSource(_configuration.ConnectionOptions.SubscriptionTimeout))
            {
                try
                {
                    var request = new SubscriptionQueryMessage
                    {
                        Id = id,
                        Type = ProtocolMessage.Request.Stop
                    };

                    using (var linkedCts = timeoutSource.GetLinkedTokenSource(_webSocketCancellationTokenSource.Token))
                    {
                        var completeTask = _incomingMessages
                            .TakeUntil(response => response.Id == id && response.Type == ProtocolMessage.Response.Complete)
                            .LastAsync()
                            .ToTask(linkedCts.Token);

                        await SendRequestAsync(request).ConfigureAwait(false);
                        await completeTask.ConfigureAwait(false);
                    }
                }
                catch (ConnectionLostException)
                {
                    // The connection was lost after the message was sent.
                    // The error would have been reported, do nothing here.
                }
                catch (OperationCanceledException)
                {
                    // We can't throw from here - the subscription is being disposed of within a RaiiAsync instance.
                    // The exception will not be observed.
                    var timeoutException = new UnsubscribeTimeoutException(id, timeoutSource.Timeout);
                    _exceptionSubject.OnNext(timeoutException);
                }
                finally
                {
                    if (removeFromRegistry)
                    {
                        _subscriptions.Remove(id);

                        // shutdown the web socket if there's no more registered subscriptions
                        if (!_subscriptions.Any())
                        {
                            ShutdownConnection();
                        }
                    }
                }
            }
        }

        private async Task SendRegistrationRequestsAsync()
        {
            var ackTasks = new List<Task>();

            foreach (var registration in _subscriptions.Values)
            {
                // registration timeout is handled
                var ackTask = SendRegistrationAsync(registration);
                ackTasks.Add(ackTask);
            }

            try
            {
                await Task.WhenAll(ackTasks).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                // it's possible for an OperationCanceledException that would not have been reported
                _exceptionSubject.OnNext(exception);
            }
        }

        private async Task<AppSyncGraphqlResponse> SendRegistrationAsync(SubscriptionRegistrationRequest registration)
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
                                response.Type is ProtocolMessage.Response.StartAck or ProtocolMessage.Response.Error)
                            .LastAsync()
                            .ToTask(linkedCts.Token);

                        await SendRequestAsync(request).ConfigureAwait(false);
                        return await ackTask.ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    // If _webSocketCancellationTokenSource was cancelled then there was a connection issue - probably wouldn't
                    // even get here unless it was the timeout token that cancelled. In reality, if _webSocketCancellationTokenSource
                    // is cancelled then the connection was most likely shutdown and an exception would have been thrown - not caught
                    // in this method.
                    if (timeoutSource.Token.IsCancellationRequested)
                    {
                        throw new SubscribeTimeoutException(registration.Id, timeoutSource.Timeout);
                    }

                    // Probably won't get here but is needed to keep the compiler happy. Worse case, the exception is reported twice.
                    throw;
                }
            }
        }

        private Task SendRequestAsync<TMessage>(TMessage message)
        {
            var buffer = _configuration.Serializer.SerializeToUtf8Bytes(message);
            var segment = new ArraySegment<byte>(buffer);

            // check if an error has occurred mid-subscription that resulted in the WebSocket being disposed
            if (_webSocket == null)
            {
                throw new ConnectionLostException();
            }

            return _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, _webSocketCancellationTokenSource.Token);
        }

        private async Task<AppSyncGraphqlResponse> GetGraphqlResponse(MemoryStream stream)
        {
            var response = await _configuration.Serializer.DeserializeObjectAsync<AppSyncGraphqlResponse>(stream, _webSocketCancellationTokenSource.Token);
            response.Message = Encoding.UTF8.GetString(stream.ToArray());

            return response;
        }
    }
}
