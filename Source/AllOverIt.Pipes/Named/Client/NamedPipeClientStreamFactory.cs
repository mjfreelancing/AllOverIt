﻿using AllOverIt.Pipes.Named.Connection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Client
{
    [ExcludeFromCodeCoverage]
    internal static class NamedPipeClientStreamFactory
    {
        public static async Task<NamedPipeReaderWriter> CreateConnectedReaderWriterAsync(string pipeName, string serverName, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            var pipeStream = await CreateConnectedStreamAsync(pipeName, serverName, timeout, cancellationToken).ConfigureAwait(false);

            return new NamedPipeReaderWriter(pipeStream, false);
        }

        public static async Task<NamedPipeClientStream> CreateConnectedStreamAsync(string pipeName, string serverName, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            var pipeStream = CreateNamedPipeClientStream(pipeName, serverName);

            try
            {
#if NET7_0_OR_GREATER
                await pipeStream
                    .ConnectAsync(timeout, cancellationToken)
                    .ConfigureAwait(false);
#else
                await pipeStream
                    .ConnectAsync((int)timeout.TotalMilliseconds, cancellationToken)
                    .ConfigureAwait(false);
#endif

                return pipeStream;
            }
            catch
            {
                await pipeStream
                    .DisposeAsync()
                    .ConfigureAwait(false);

                throw;
            }
        }

        private static NamedPipeClientStream CreateNamedPipeClientStream(string pipeName, string serverName)
        {
            return new NamedPipeClientStream(
                serverName,
                pipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }
    }
}