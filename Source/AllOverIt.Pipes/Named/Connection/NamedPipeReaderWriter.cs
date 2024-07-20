using AllOverIt.Assertion;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Named.Connection
{
    // Composes a reader and writer
    internal sealed class NamedPipeReaderWriter : IAsyncDisposable
    {
        private readonly NamedPipeStreamReader _streamReader;
        private readonly NamedPipeStreamWriter _streamWriter;
        private PipeStream? _pipeStream;

        public NamedPipeReaderWriter(PipeStream stream)
        {
            _pipeStream = stream.WhenNotNull();   // This class assumes ownership

            _streamReader = new NamedPipeStreamReader(_pipeStream);
            _streamWriter = new NamedPipeStreamWriter(_pipeStream);
        }

        public Task<byte[]> ReadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_pipeStream is null)
            {
                throw new IOException("The underlying pipe stream has been closed.");
            }

            return _streamReader.ReadAsync(cancellationToken);
        }

        public Task WriteAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_pipeStream is null)
            {
                throw new IOException("The underlying pipe stream has been closed.");
            }

            // Writes, flushes, and waits for the pipe to drain - _streamWriter checks for a broken connection
            return _streamWriter.WriteAsync(buffer, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            if (_pipeStream is not null)
            {
                try
                {
                    await _pipeStream.DisposeAsync().ConfigureAwait(false);
                }
                catch (ObjectDisposedException)
                {
                    // The pipe stream may have already been disposed due to a broken pipe.
                }
                finally
                {
                    _pipeStream = null;
                }
            }
        }
    }
}