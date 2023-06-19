﻿using AllOverIt.Assertion;
using AllOverIt.Async;
using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Named.Serialization;
using System;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Connection
{
    internal abstract class NamedPipeConnection<TMessage> : IConnectableNamedPipeConnection<TMessage>
    {
        private readonly INamedPipeSerializer<TMessage> _serializer;

        private CancellationTokenSource _cancellationTokenSource;
        private BackgroundTask _backgroundReader;
        private NamedPipeReaderWriter _pipeReaderWriter;

        // Will be a NamedPipeClientStream or NamedPipeServerStream
        protected PipeStream PipeStream { get; private set; }

        /// <inheritdoc />
        public string PipeName { get; }

        /// <inheritdoc />
        public bool IsConnected => PipeStream?.IsConnected ?? false;

        internal NamedPipeConnection(PipeStream stream, string pipeName, INamedPipeSerializer<TMessage> serializer)
        {
            PipeStream = stream.WhenNotNull(nameof(stream));               // Assume ownership of this stream
            PipeName = pipeName.WhenNotNullOrEmpty(nameof(pipeName));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        /// <inheritdoc />
        /// <remarks>A connection cannot be re-established after it has been disconnected.</remarks>
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

                        if (!bytes.Any() || !IsConnected)
                        {
                            break;
                        }

                        var @object = _serializer.Deserialize(bytes);

                        DoOnMessageReceived(@object);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex)    // IOException or any other
                    {
                        // PipeStreamReader will throw IOException if an expected byte count is not received.
                        // This can occur if the connection is killed during communication. Fall through so
                        // the connection is treated as disconnected.


                        // Make sure the PipeStream and associated reader/writer are disposed so IsConnected is reported as false
                        await DisposePipeStreamResources().ConfigureAwait(false);

                        DoOnException(ex);
                    }
                }

                DoOnDisconnected();
            }, _cancellationTokenSource.Token);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task WriteAsync(TMessage value, CancellationToken cancellationToken = default)
        {
            Throw<PipeException>.When(!IsConnected, "Named pipe connection is not connected.");
            Throw<PipeException>.When(!_pipeReaderWriter.CanWrite, "Named pipe connection is not writable.");

            cancellationToken.ThrowIfCancellationRequested();

            var bytes = _serializer.Serialize(value);

            await _pipeReaderWriter.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync().ConfigureAwait(false);
        }

        protected abstract void DoOnDisconnected();
        protected abstract void DoOnMessageReceived(TMessage message);
        protected abstract void DoOnException(Exception exception);

        private async Task DisposePipeStreamResources()
        {
            await _pipeReaderWriter.DisposeAsync().ConfigureAwait(false);
            _pipeReaderWriter = null;

            await PipeStream.DisposeAsync().ConfigureAwait(false);
            PipeStream = null;
        }
    }
}