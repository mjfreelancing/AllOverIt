using AllOverIt.Assertion;
using AllOverIt.Async;
using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Named.Serialization;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Named.Connection
{
    internal abstract class NamedPipeConnection<TMessage> : IConnectableNamedPipeConnection<TMessage>
    {
        private bool _disposed;

        private readonly INamedPipeSerializer<TMessage> _serializer;
        private readonly NamedPipeReaderWriter _pipeReaderWriter;

        // Will be a NamedPipeClientStream or NamedPipeServerStream. This class assumes ownership until _pipeReaderWriter is created.
        private readonly PipeStream _pipeStream;

        private CancellationTokenSource? _cancellationTokenSource;
        private BackgroundTask? _backgroundReader;
        private bool _isConnected;

        private bool PipeStreamConnected => _pipeStream.IsConnected;

        protected NamedPipeServerStream? ServerStream => _pipeStream as NamedPipeServerStream;

        public string ConnectionId { get; }

        public bool IsConnected => _isConnected && PipeStreamConnected;

        internal NamedPipeConnection(PipeStream pipeStream, string connectionId, INamedPipeSerializer<TMessage> serializer)
        {
            _pipeStream = pipeStream.WhenNotNull(nameof(pipeStream));               // Assume ownership of this stream
            ConnectionId = connectionId.WhenNotNullOrEmpty(nameof(connectionId));
            _serializer = serializer.WhenNotNull(nameof(serializer));

            _pipeReaderWriter = new NamedPipeReaderWriter(_pipeStream);
        }

        public void Connect()
        {
            Throw<PipeException>.When(_isConnected, "The named pipe connection is already open.");

            _cancellationTokenSource = new CancellationTokenSource();

            _backgroundReader = new BackgroundTask(async cancellationToken =>
            {
                while (!cancellationToken.IsCancellationRequested && PipeStreamConnected)
                {
                    try
                    {
                        var bytes = await _pipeReaderWriter
                            .ReadAsync(cancellationToken)
                            .ConfigureAwait(false);

                        if (bytes.Length == 0 || !IsConnected)
                        {
                            break;
                        }

                        var message = _serializer.Deserialize(bytes)!;

                        DoOnMessageReceived(message);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception exception)
                    {
                        if (exception is IOException or ObjectDisposedException)
                        {
                            // IOException
                            // PipeStreamReader will throw IOException if an expected byte count is not received.
                            // This can occur if the connection is killed during communication. Fall through so
                            // the connection is treated as disconnected.

                            // ObjectDisposedException
                            // Can be thrown if the underlying pipe stream is closed. Allow to fall through so
                            // the connection is treated as disconnected.
                            exception = new PipeException("Cannot read from the pipe as it is not available.", exception);
                        }

                        DoOnException(exception);

                        break;
                    }
                }

                // Cannot call DisconnectAsync() here as this will attempt to cancel / dispose of this background worker thread and cause a deadlock.

                try
                {
                    _isConnected = false;

                    DoOnDisconnected();
                }
                catch (Exception exception)
                {
                    DoOnException(exception);
                }
            }, _cancellationTokenSource.Token);

            _isConnected = true;
        }

        public async Task DisconnectAsync()
        {
            // Cannot return early based on !_isConnected since this may be set if the background reader terminates early due to a broken pipe

            // _backgroundReader may, or may not, be running - so dispose of related resources as required
            if (_cancellationTokenSource is not null && !_cancellationTokenSource!.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }

            if (_backgroundReader is not null)
            {
                await _backgroundReader!.DisposeAsync().ConfigureAwait(false);
                _backgroundReader = null;
            }

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            _isConnected = false;
        }

        public async Task WriteAsync(TMessage value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // The connection can be broken at any time
            if (IsConnected)
            {
                var bytes = _serializer.Serialize(value);

                await _pipeReaderWriter!.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            await DisconnectAsync().ConfigureAwait(false);

            // This will also dispose PipeStream as it assumes ownership once created
            await _pipeReaderWriter.DisposeAsync().ConfigureAwait(false);

            _disposed = true;
        }

        protected abstract void DoOnDisconnected();
        protected abstract void DoOnMessageReceived(TMessage message);
        protected abstract void DoOnException(Exception exception);
    }
}