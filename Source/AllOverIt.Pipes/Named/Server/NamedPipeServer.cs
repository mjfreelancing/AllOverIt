using AllOverIt.Assertion;
using AllOverIt.Async;
using AllOverIt.Extensions;
using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Serialization;
using AllOverIt.Threading;
using AllOverIt.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Server
{
    /// <summary>A named pipe server that can broadcast a strongly type message to all connected clients
    /// as well as receive messages from those clients.</summary>
    /// <typeparam name="TMessage"></typeparam>
    public sealed class NamedPipeServer<TMessage> : INamedPipeServer<TMessage> where TMessage : class, new()
    {
        private readonly INamedPipeSerializer<TMessage> _serializer;
        internal IList<INamedPipeServerConnection<TMessage>> Connections { get; } = new List<INamedPipeServerConnection<TMessage>>();
        private AwaitableLock _connectionsLock = new();
        private BackgroundTask _backgroundTask;

        /// <inheritdoc />
        public string PipeName { get; }

        /// <inheritdoc />
        public bool IsStarted
        {
            get
            {
                Task task = _backgroundTask;

                return task is not null &&
                    !task.IsCompleted &&
                    !task.IsCanceled &&
                    !task.IsFaulted;
            }
        }

        /// <summary>An event raised when a client connects to the server. This event may be raised on a background thread.</summary>
        public event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>> OnClientConnected;

        /// <summary>An event raised when a client disconnects from the server. This event may be raised on a background thread.</summary>
        public event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>> OnClientDisconnected;

        /// <summary>An event raised when a client sends a message to the server. This event may be raised on a background thread.</summary>
        public event EventHandler<NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeServerConnection<TMessage>>> OnMessageReceived;

        /// <summary>An event raised when an exception is thrown during a read or write operation.</summary>
        public event EventHandler<NamedPipeExceptionEventArgs> OnException;

        /// <inheritdoc />
        public NamedPipeServer(string pipeName, INamedPipeSerializer<TMessage> serializer)
        {
            PipeName = pipeName.WhenNotNullOrEmpty(nameof(pipeName));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        /// <inheritdoc />
        public void Start(Action<PipeSecurity> pipeSecurity)
        {
            _ = pipeSecurity.WhenNotNull(nameof(pipeSecurity));

            var security = new PipeSecurity();

            pipeSecurity.Invoke(security);

            Start(security);
        }

        /// <inheritdoc />
        public void Start(PipeSecurity pipeSecurity = default)
        {
            Throw<PipeException>.WhenNotNull(_backgroundTask, "The named pipe server has already been started.");

            _backgroundTask = new BackgroundTask(async token =>
            {
                while (!token.IsCancellationRequested)
                {
                    NamedPipeServerConnection<TMessage> connection = null;

                    try
                    {
                        var connectionId = $"{Guid.NewGuid()}";

                        // Send the client the name of the data pipe to use
                        var serverStream = NamedPipeServerStreamFactory.CreateStream(PipeName, pipeSecurity);       // TODO: Allow the factory to provide default security, this will override if not null

                        await using (serverStream)
                        {
                            await serverStream.WaitForConnectionAsync(token).ConfigureAwait(false);

                            await using (var writer = new NamedPipeReaderWriter(serverStream, false))
                            {
                                await writer
                                    .WriteAsync(Encoding.UTF8.GetBytes(connectionId), token)
                                    .ConfigureAwait(false);
                            }
                        }

                        // Wait for the client to connect to the data pipe
                        var connectionStream = NamedPipeServerStreamFactory.CreateStream(connectionId, pipeSecurity);

                        await connectionStream.WaitForConnectionAsync(token).ConfigureAwait(false);

                        // Add the client's connection to the list of connections
                        connection = new NamedPipeServerConnection<TMessage>(connectionStream, connectionId, _serializer);

                        connection.OnMessageReceived += DoOnConnectionMessageReceived;
                        connection.OnDisconnected += DoOnConnectionDisconnected;
                        connection.OnException += DoOnConnectionException;

                        connection.Connect();

                        await DoOnClientConnectedAsync(connection);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception exception)
                    {
                        if (connection is not null)
                        {
                            await connection.DisposeAsync().ConfigureAwait(false);
                        }

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

            // We cannot lock here since the OnDisconnected event handler will be called - and apply the lock
            // New connections are not possible now the background thread has been disposed of.

            if (Connections.Any())
            {
                // DoOnConnectionDisconnected() will be called for each connection where
                // its' event handlers are also released.
                await Connections.DisposeAllAsync().ConfigureAwait(false);
            }
        }

        /// <summary>Asynchronously sends a message to all connected clients.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        public Task WriteAsync(TMessage message, CancellationToken cancellationToken = default)
        {
            return WriteAsync(message, _ => true, cancellationToken);
        }

        /// <summary>Asynchronously sends a message to the client with a specfied pipe name..</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="pipeName">The name of the pipe to send the message to. This name is case-insensitive.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        public Task WriteAsync(TMessage message, string pipeName, CancellationToken cancellationToken = default)
        {
            _ = pipeName.WhenNotNullOrEmpty(nameof(pipeName));

            return WriteAsync(message, connection => connection.ConnectionId.Equals(pipeName, StringComparison.OrdinalIgnoreCase), cancellationToken);
        }

        /// <summary>Asynchronously sends a message to all connected clients that meet a predicate condition.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="predicate">The predicate condition to be met.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        public async Task WriteAsync(TMessage message, Func<INamedPipeConnection<TMessage>, bool> predicate, CancellationToken cancellationToken = default)
        {
            _ = message.WhenNotNull(nameof(message));
            _ = predicate.WhenNotNull(nameof(predicate));

            cancellationToken.ThrowIfCancellationRequested();

            // Get potential connections synchronously
            IEnumerable<INamedPipeConnection<TMessage>> connectionsSnapshot = null;

            using (await _connectionsLock.GetLockAsync(cancellationToken))
            {
                connectionsSnapshot = Connections
                    .Where(connection => connection.IsConnected && predicate.Invoke(connection))
                    .AsReadOnlyCollection();
            }

            foreach (var connection in connectionsSnapshot)
            {
                try
                {
                    await connection.WriteAsync(message, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    DoOnException(exception);
                }
            }
        }

        /// <summary>Stops the pipe server and releases resources.</summary>
        public async ValueTask DisposeAsync()
        {
            if (_connectionsLock is not null)
            {
                await StopAsync().ConfigureAwait(false);

                _connectionsLock.Dispose();
                _connectionsLock = null;
            }
        }

        private async Task DoOnClientConnectedAsync(INamedPipeServerConnection<TMessage> connection)
        {
            using (await _connectionsLock.GetLockAsync().ConfigureAwait(false))
            {
                Connections.Add(connection);
            }

            var clientConnected = OnClientConnected;

            if (clientConnected is not null)
            {
                var args = new NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>(connection);

                clientConnected.Invoke(this, args);
            }
        }

        private void DoOnConnectionMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeServerConnection<TMessage>> args)
        {
            OnMessageReceived?.Invoke(this, args);
        }

        private async void DoOnConnectionDisconnected(object sender, NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>> args)
        {
            OnClientDisconnected?.Invoke(this, args);

            try
            {
                var connection = args.Connection;

                connection.OnMessageReceived -= DoOnConnectionMessageReceived;
                connection.OnDisconnected -= DoOnConnectionDisconnected;
                connection.OnException -= DoOnConnectionException;

                using (await _connectionsLock.GetLockAsync().ConfigureAwait(false))
                {
                    Connections.Remove(connection);
                }
            }
            catch (Exception exception)
            {
                DoOnException(exception);
            }
        }

        private void DoOnConnectionException(object sender, NamedPipeConnectionExceptionEventArgs<TMessage, INamedPipeServerConnection<TMessage>> args)
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
    }
}