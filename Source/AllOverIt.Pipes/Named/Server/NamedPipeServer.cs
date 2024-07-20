using AllOverIt.Assertion;
using AllOverIt.Async;
using AllOverIt.Collections;
using AllOverIt.Collections.Extensions;
using AllOverIt.Extensions;
using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Serialization;
using System.IO.Pipes;
using System.Text;

namespace AllOverIt.Pipes.Named.Server
{
    /// <summary>A named pipe server that can broadcast a strongly type message to all connected clients
    /// as well as receive messages from those clients.</summary>
    /// <typeparam name="TMessage"></typeparam>
    public sealed class NamedPipeServer<TMessage> : INamedPipeServer<TMessage> where TMessage : class, new()
    {
        private bool _disposed;
        private readonly INamedPipeSerializer<TMessage> _serializer;
        private BackgroundTask? _backgroundTask;

        internal LockableList<INamedPipeServerConnection<TMessage>> _connections = new(true);

        /// <inheritdoc />
        public string PipeName { get; }

        /// <inheritdoc />
        public bool IsStarted
        {
            get
            {
                Task? task = _backgroundTask;

                return task is not null &&
                    !task.IsCompleted &&
                    !task.IsCanceled &&
                    !task.IsFaulted;
            }
        }

        /// <summary>An event raised when a client connects to the server. This event may be raised on a background thread.</summary>
        public event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>>? OnClientConnected;

        /// <summary>An event raised when a client disconnects from the server. This event may be raised on a background thread.</summary>
        public event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>>? OnClientDisconnected;

        /// <summary>An event raised when a client sends a message to the server. This event may be raised on a background thread.</summary>
        public event EventHandler<NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeServerConnection<TMessage>>>? OnMessageReceived;

        /// <summary>An event raised when an exception is thrown during a connection or subsequent read or write operation.</summary>
        public event EventHandler<NamedPipeExceptionEventArgs>? OnException;

        /// <inheritdoc />
        public NamedPipeServer(string pipeName, INamedPipeSerializer<TMessage> serializer)
        {
            PipeName = pipeName.WhenNotNullOrEmpty();
            _serializer = serializer.WhenNotNull();
        }

        /// <inheritdoc />
        public void Start(Action<PipeSecurity> pipeSecurity)
        {
            _ = pipeSecurity.WhenNotNull();

            Throw<PipeException>.WhenNotNull(_backgroundTask, "The named pipe server has already been started.");

            var security = new PipeSecurity();

            pipeSecurity.Invoke(security);

            Start(security);
        }

        /// <inheritdoc />
        public void Start(PipeSecurity? pipeSecurity = default)
        {
            Throw<PipeException>.WhenNotNull(_backgroundTask, "The named pipe server has already been started.");

            _backgroundTask = new BackgroundTask(async cancellationToken =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var connectionId = $"{Guid.NewGuid()}";

                        // Send the client the name of the data pipe to use
                        var serverStream = NamedPipeServerStreamFactory.CreateStream(PipeName, pipeSecurity);       // TODO: Allow the factory to provide default security, this will override if not null

                        await using (serverStream)
                        {
                            await serverStream
                                .WaitForConnectionAsync(cancellationToken)
                                .ConfigureAwait(false);

                            await using var writer = new NamedPipeReaderWriter(serverStream);

                            await writer
                                .WriteAsync(Encoding.UTF8.GetBytes(connectionId), cancellationToken)
                                .ConfigureAwait(false);
                        }

                        // Wait for the client to connect to the data pipe.
                        var connectionStream = NamedPipeServerStreamFactory.CreateStream(connectionId, pipeSecurity);

                        await connectionStream
                            .WaitForConnectionAsync(cancellationToken)
                            .ConfigureAwait(false);

                        // Add the client's connection to the list of connections.
                        await CreateActiveServerConnectionAsync(connectionStream, connectionId, cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception exception)
                    {
                        DoOnException(exception);
                    }
                }
            });
        }

        /// <inheritdoc />
        public async Task StopAsync()
        {
            // Prevent new connections from being established
            if (_backgroundTask is not null)
            {
                await _backgroundTask.DisposeAsync().ConfigureAwait(false);
                _backgroundTask = null;
            }

            // New connections cannot be added once _backgroundTask has closed down, but there's always the chance that
            // an existing connection will be dropped while this disconnection processing is in play. Doing our best to
            // avoid issues, but there is still a chance of a race condition if the client side terminates preemptively.
            // The named pipe connection is thread safe and idempotent if disposed of multiple times.
            List<INamedPipeServerConnection<TMessage>> currentConnections = _connections.Clone();

            if (currentConnections.Count > 0)
            {
                // OnDisconnect() will be called for each
                var tasks = currentConnections.SelectToList(connection => connection.DisconnectAsync());

                await Task.WhenAll(tasks).ConfigureAwait(false);

                await currentConnections.DisposeAllAsync().ConfigureAwait(false);
            }
        }

        /// <summary>Asynchronously sends a message to all connected clients.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        public Task WriteAsync(TMessage message, CancellationToken cancellationToken = default)
        {
            return WriteAsync(message, _ => true, cancellationToken);
        }

        /// <summary>Asynchronously sends a message to the client with a specified pipe name..</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="pipeName">The name of the pipe to send the message to. This name is case-insensitive.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        public Task WriteAsync(TMessage message, string pipeName, CancellationToken cancellationToken = default)
        {
            _ = pipeName.WhenNotNullOrEmpty();

            return WriteAsync(message, connection => connection.ConnectionId.Equals(pipeName, StringComparison.OrdinalIgnoreCase), cancellationToken);
        }

        /// <summary>Asynchronously sends a message to all connected clients that meet a predicate condition.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="predicate">The predicate condition to be met.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        public async Task WriteAsync(TMessage message, Func<INamedPipeConnection<TMessage>, bool> predicate, CancellationToken cancellationToken = default)
        {
            _ = message.WhenNotNull();
            _ = predicate.WhenNotNull();

            cancellationToken.ThrowIfCancellationRequested();

            INamedPipeConnection<TMessage>[] targetConnections;

            using (_connections.GetReadLock(false))
            {
                targetConnections = _connections.Where(predicate).ToArray();
            }

            List<Exception>? exceptions = [];

            foreach (var connection in targetConnections)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    if (connection.IsConnected)
                    {
                        await connection.WriteAsync(message, cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception exception)
                {
                    if (exception is IOException or ObjectDisposedException)
                    {
                        // IOException              - A broken pipe, such as due to the server terminating
                        // ObjectDisposedException  - Cannot access a closed pipe
                        exception = new PipeException("Cannot write to the pipe as it is not available.", exception);
                    }

                    exceptions.Add(exception);
                }
            }

            if (exceptions.Count != 0)
            {
                var aggregate = new AggregateException("One or more named pipe connections failed to write the message", exceptions);

                DoOnException(aggregate);
            }
        }

        /// <summary>Stops the pipe server and releases resources.</summary>
        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            // We cannot lock here since the OnDisconnected event handler will be called
            await StopAsync().ConfigureAwait(false);

            _disposed = true;
        }

        private async Task CreateActiveServerConnectionAsync(NamedPipeServerStream connectionStream, string connectionId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var connection = new NamedPipeServerConnection<TMessage>(connectionStream, connectionId, _serializer);

            AddConnection(connection);

            try
            {
                connection.Connect();

                DoOnClientConnected(connection, cancellationToken);
            }
            catch
            {
                RemoveConnection(connection);

                await connection.DisposeAsync().ConfigureAwait(false);

                throw;  // The caller will call DoOnException(exception);
            }
        }

        private void DoOnClientConnected(INamedPipeServerConnection<TMessage> connection, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var clientConnected = OnClientConnected;

            if (clientConnected is not null)
            {
                var args = new NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>(connection);

                clientConnected.Invoke(this, args);
            }
        }

        private void DoOnConnectionMessageReceived(object? sender, NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeServerConnection<TMessage>> args)
        {
            OnMessageReceived?.Invoke(this, args);
        }

        private void DoOnConnectionDisconnected(object? sender, NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>> args)
        {
            try
            {
                var connection = args.Connection;

                RemoveConnection(connection);

                OnClientDisconnected?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                DoOnException(exception);
            }
        }

        private void DoOnConnectionException(object? sender, NamedPipeConnectionExceptionEventArgs<TMessage, INamedPipeServerConnection<TMessage>> args)
        {
            DoOnException(args.Exception);
        }

        private void DoOnException(Exception exception)
        {
            var onException = OnException;

            if (onException is not null)
            {
                var args = new NamedPipeExceptionEventArgs(exception);

                onException.Invoke(this, args);
            }
        }

        private void AddConnection(INamedPipeServerConnection<TMessage> connection)
        {
            connection.OnMessageReceived += DoOnConnectionMessageReceived;
            connection.OnDisconnected += DoOnConnectionDisconnected;
            connection.OnException += DoOnConnectionException;

            _connections.Add(connection);
        }

        private void RemoveConnection(INamedPipeServerConnection<TMessage> connection)
        {
            connection.OnMessageReceived -= DoOnConnectionMessageReceived;
            connection.OnDisconnected -= DoOnConnectionDisconnected;
            connection.OnException -= DoOnConnectionException;

            _connections.Remove(connection);
        }
    }
}