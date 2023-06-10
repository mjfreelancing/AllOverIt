﻿using AllOverIt.Assertion;
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Connection
{
    /// <summary>Provides the ability to read buffered data from an underlying pipe stream.</summary>
    public sealed class PipeStreamReader
    {
        private readonly PipeStream _pipeStream;

        /// <summary>Constructor.</summary>
        /// <param name="pipeStream">The pipe stream to read from.</param>
        public PipeStreamReader(PipeStream pipeStream)
        {
            _pipeStream = pipeStream.WhenNotNull(nameof(pipeStream));
        }

        /// <summary>Reads an array of bytes from the pipe stream. This method waits until bytes are received
        /// or the pipe stream is disconnected.</summary>
        /// <returns>An array of bytes read from the pipe stream. This array will be empty if the pipe stream
        /// is disconnected.</returns>
        public async Task<byte[]> ReadAsync(CancellationToken cancellationToken)
        {
            var length = await ReadLengthAsync(cancellationToken).ConfigureAwait(false);

            return length == 0
                ? Array.Empty<byte>()
                : await ReadAsync(length, true, cancellationToken).ConfigureAwait(false);
        }

        private async Task<int> ReadLengthAsync(CancellationToken cancellationToken)
        {
            var bytes = await ReadAsync(sizeof(int), false, cancellationToken).ConfigureAwait(false);

            if (!bytes.Any())
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
                    : Array.Empty<byte>();
            }

            return buffer;
        }
    }
}