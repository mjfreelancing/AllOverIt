using AllOverIt.Assertion;
using System.IO.Pipes;
using System.Net;

namespace AllOverIt.Pipes.Named.Connection
{
    // Reads buffered data from an underlying pipe stream
    internal sealed class NamedPipeStreamReader
    {
        private readonly PipeStream _pipeStream;

        public NamedPipeStreamReader(PipeStream pipeStream)
        {
            _pipeStream = pipeStream.WhenNotNull(nameof(pipeStream));
        }

        // Reads an array of bytes from the pipe stream. This method waits until bytes are received or the pipe stream is disconnected.
        public async Task<byte[]> ReadAsync(CancellationToken cancellationToken)
        {
            var length = await ReadLengthAsync(cancellationToken).ConfigureAwait(false);

            return length == 0
                ? []
                : await ReadAsync(length, true, cancellationToken).ConfigureAwait(false);
        }

        private async Task<int> ReadLengthAsync(CancellationToken cancellationToken)
        {
            var bytes = await ReadAsync(sizeof(int), false, cancellationToken).ConfigureAwait(false);

            if (bytes.Length == 0)
            {
                return 0;
            }

            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
        }

        private async Task<byte[]> ReadAsync(int length, bool throwIfInsufficientBytes, CancellationToken cancellationToken)
        {
            var buffer = new byte[length];

            var bytesRead = await _pipeStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            // This can occur if the connection is killed mid-communication
            if (bytesRead != length)
            {
                return throwIfInsufficientBytes
                    ? throw new IOException($"Expected {length} bytes but read {bytesRead}")
                    : [];
            }

            return buffer;
        }
    }
}