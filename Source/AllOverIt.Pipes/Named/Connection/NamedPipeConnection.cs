using AllOverIt.Assertion;
using AllOverIt.Async;
using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Named.Serialization;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Named.Connection
{
    internal abstract class NamedPipeConnection<TMessage> : IConnectableNamedPipeConnection<TMessage>
    {
        private readonly INamedPipeSerializer<TMessage> _serializer;

        private CancellationTokenSource? _cancellationTokenSource;
        private BackgroundTask? _backgroundReader;
        private NamedPipeReaderWriter? _pipeReaderWriter;

        private bool IsRunning =>
            _cancellationTokenSource is not null &&
            _pipeReaderWriter is not null &&
            _backgroundReader is not null;

        public string ConnectionId { get; }

        public bool IsConnected => IsRunning && (PipeStream?.IsConnected ?? false);

        // Will be a NamedPipeClientStream or NamedPipeServerStream. This class assumes ownership until _pipeReaderWriter is created.
        protected PipeStream? PipeStream { get; private set; }

        internal NamedPipeConnection(PipeStream pipeStream, string connectionId, INamedPipeSerializer<TMessage> serializer)
        {
            PipeStream = pipeStream.WhenNotNull(nameof(pipeStream));               // Assume ownership of this stream
            ConnectionId = connectionId.WhenNotNullOrEmpty(nameof(connectionId));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        // Note: A connection cannot be re-established after it has been disconnected
        public void Connect()
        {
            Throw<PipeException>.When(IsRunning, "The named pipe connection is already open.");
            Throw<PipeException>.WhenNull(PipeStream, "The named pipe connection cannot be opened after it has been disconnected.");

            _cancellationTokenSource = new CancellationTokenSource();

            _pipeReaderWriter = new NamedPipeReaderWriter(PipeStream);

            _backgroundReader = new BackgroundTask(async cancellationToken =>
            {
                while (!cancellationToken.IsCancellationRequested && IsConnected)
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

                        var message = _serializer.Deserialize(bytes);

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

                // Dispose of the PipeStream and associated reader/writer - cannot call DisconnectAsync() here
                // as this will attempt to cancel / dispose of this background worker thread and cause a deadlock.
                await DisposeResources().ConfigureAwait(false);

                try
                {
                    DoOnDisconnected();
                }
                catch (Exception exception)
                {
                    DoOnException(exception);
                }
            }, _cancellationTokenSource.Token);
        }

        public async Task DisconnectAsync()
        {
            _cancellationTokenSource?.Cancel();

            if (_backgroundReader is not null)
            {
                await _backgroundReader!.DisposeAsync().ConfigureAwait(false);
                _backgroundReader = null;
            }

            await DisposeResources().ConfigureAwait(false);
        }

        public async Task WriteAsync(TMessage value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // The connection can be broken at any time
            if (IsConnected && IsRunning)
            {
                var bytes = _serializer.Serialize(value);

                await _pipeReaderWriter!.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync().ConfigureAwait(false);
        }

        protected abstract void DoOnDisconnected();
        protected abstract void DoOnMessageReceived(TMessage message);
        protected abstract void DoOnException(Exception exception);

        private async Task DisposeResources()
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            if (_pipeReaderWriter is not null)
            {
                // This will also dispose PipeStream as it assumes ownership once created
                await _pipeReaderWriter.DisposeAsync().ConfigureAwait(false);
                _pipeReaderWriter = null;

                PipeStream = null;
            }

            if (PipeStream is not null)
            {
                await PipeStream!.DisposeAsync().ConfigureAwait(false);
                PipeStream = null;
            }
        }
    }
}