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

        private CancellationTokenSource _cancellationTokenSource;
        private BackgroundTask _backgroundReader;
        private NamedPipeReaderWriter _pipeReaderWriter;

        public string ConnectionId { get; }

        public bool IsConnected => PipeStream?.IsConnected ?? false;

        // Will be a NamedPipeClientStream or NamedPipeServerStream
        protected PipeStream PipeStream { get; private set; }

        internal NamedPipeConnection(PipeStream pipeStream, string connectionId, INamedPipeSerializer<TMessage> serializer)
        {
            PipeStream = pipeStream.WhenNotNull(nameof(pipeStream));               // Assume ownership of this stream
            ConnectionId = connectionId.WhenNotNullOrEmpty(nameof(connectionId));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        // Note: A connection cannot be re-established after it has been disconnected
        public void Connect()
        {
            Throw<PipeException>.WhenNotNull(_backgroundReader, "The named pipe connection is already open.");
            Throw<PipeException>.WhenNull(PipeStream, "The named pipe connection cannot be opened after it has been disconnected.");

            _cancellationTokenSource = new CancellationTokenSource();

            _pipeReaderWriter = new NamedPipeReaderWriter(PipeStream, true);

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
                    catch (Exception exception)    // IOException or any other
                    {
                        // PipeStreamReader will throw IOException if an expected byte count is not received.
                        // This can occur if the connection is killed during communication. Fall through so
                        // the connection is treated as disconnected.

                        DoOnException(exception);

                        break;
                    }
                }

                if (IsConnected)
                {
                    // Dispose of the PipeStream and associated reader/writer - cannot call DisconnectAsync() here
                    // as this will attempt to cancel / dispose of this background worker thread and cause a deadlock.
                    await DisposePipeStreamResources().ConfigureAwait(false);
                }

                DoOnDisconnected();
            }, _cancellationTokenSource.Token);
        }

        public async Task DisconnectAsync()
        {
            if (_cancellationTokenSource is not null)
            {
                _cancellationTokenSource.Cancel();

                await _backgroundReader.DisposeAsync().ConfigureAwait(false);
                _backgroundReader = null;

                await DisposePipeStreamResources().ConfigureAwait(false);

                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public async Task WriteAsync(TMessage value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // The connection can be broken at any time
            if (IsConnected)
            {
                var bytes = _serializer.Serialize(value);

                await _pipeReaderWriter.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync().ConfigureAwait(false);
        }

        protected abstract void DoOnDisconnected();
        protected abstract void DoOnMessageReceived(TMessage message);
        protected abstract void DoOnException(Exception exception);

        private async Task DisposePipeStreamResources()
        {
            if (_pipeReaderWriter is not null)
            {
                await _pipeReaderWriter.DisposeAsync().ConfigureAwait(false);
                _pipeReaderWriter = null;

                await PipeStream.DisposeAsync().ConfigureAwait(false);
                PipeStream = null;
            }
        }
    }
}