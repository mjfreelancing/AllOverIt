using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Connection
{
    /// <summary>Represents and named pipe that can write a message to an underlying pipe stream.</summary>
    /// <typeparam name="TMessage">The message type serialized between a named pipe client and a named pipe server.</typeparam>
    public interface INamedPipe<TMessage> where TMessage : class, new()
    {
        /// <summary>Asynchronously sends a message to all connected clients.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        Task WriteAsync(TMessage message, CancellationToken cancellationToken = default);
    }
}