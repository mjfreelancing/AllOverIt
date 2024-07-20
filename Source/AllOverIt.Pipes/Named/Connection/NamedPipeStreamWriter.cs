using AllOverIt.Assertion;
using System.IO.Pipes;
using System.Net;

namespace AllOverIt.Pipes.Named.Connection
{
    // Writes buffered data to an underlying pipe stream
    internal sealed class NamedPipeStreamWriter
    {
        private readonly PipeStream _pipeStream;

        private bool IsConnected => _pipeStream?.IsConnected ?? false;

        public NamedPipeStreamWriter(PipeStream pipeStream)
        {
            _pipeStream = pipeStream.WhenNotNull();
        }

        public async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            _ = buffer.WhenNotNullOrEmpty();

            cancellationToken.ThrowIfCancellationRequested();

            if (!IsConnected)
            {
                return;
            }

            var lengthBuffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(buffer.Length));

            await _pipeStream!.WriteAsync(lengthBuffer, cancellationToken).ConfigureAwait(false);
            await _pipeStream!.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
            await _pipeStream.FlushAsync(cancellationToken).ConfigureAwait(false);

            _pipeStream.WaitForPipeDrain();
        }
    }
}