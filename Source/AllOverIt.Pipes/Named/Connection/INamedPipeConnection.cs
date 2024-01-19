using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Connection
{
    /// <summary>A connection for an underlying <see cref="System.IO.Pipes.PipeStream"/> that can serialize messages.</summary>
    /// <typeparam name="TMessage">The message type serialized by the connection.</typeparam>
    public interface INamedPipeConnection<TMessage> : IAsyncDisposable
    {
        /// <summary>Gets the connection's unique identifier.</summary>
        public string ConnectionId { get; }

        /// <summary>Indicates if the underlying pipe stream is connected.</summary>
        public bool IsConnected { get; }

        /// <summary>Serializes a provided <paramref name="message"/> and waits for the other end of the connection
        /// to read it.</summary>
        /// <param name="message">The message to be serialized to the pipe.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task WriteAsync(TMessage message, CancellationToken cancellationToken = default);
    }
}