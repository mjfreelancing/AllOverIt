using AllOverIt.Assertion;
using AllOverIt.Pipes.Exceptions;
using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Connection
{
    // Composes a reader and writer
    internal sealed class NamedPipeReaderWriter : IAsyncDisposable
    {
        private readonly bool _leaveConnected;
        private readonly NamedPipeStreamReader _streamReader;
        private readonly NamedPipeStreamWriter _streamWriter;
        private PipeStream _pipeStream;

        public NamedPipeReaderWriter(PipeStream stream, bool leaveConnected)
        {
            _pipeStream = stream.WhenNotNull(nameof(stream));
            _leaveConnected = leaveConnected;

            _streamReader = new NamedPipeStreamReader(_pipeStream);
            _streamWriter = new NamedPipeStreamWriter(_pipeStream);
        }

        public Task<byte[]> ReadAsync(CancellationToken cancellationToken = default)
        {
            // An empty array will be returned if the connection has been broken
            return _streamReader.ReadAsync(cancellationToken);
        }

        public Task WriteAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Writes, flushes, and waits for the pipe to drain - _streamWriter checks for a broken connection
            return _streamWriter.WriteAsync(buffer, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            if (!_leaveConnected && _pipeStream is not null)
            {
                await _pipeStream.DisposeAsync().ConfigureAwait(false);
                _pipeStream = null;
            }
        }
    }
}