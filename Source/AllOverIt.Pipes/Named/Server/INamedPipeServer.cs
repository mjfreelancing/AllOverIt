using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Named.Server
{
    /// <summary>Represents a named pipe server that can broadcast a strongly type message to all connected clients
    /// as well as receive messages from those clients.</summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    public interface INamedPipeServer<TMessage> : INamedPipe<TMessage>, INamedPipeEvents<TMessage>, INamedPipeServerEvents<TMessage>, IAsyncDisposable
        where TMessage : class, new()
    {
        /// <summary>The name of the pipe.</summary>
        string PipeName { get; }

        /// <summary>Indicates if the server has started and is actively allowing client connections.</summary>
        bool IsStarted { get; }

        /// <summary>Starts listening for new pipe client connections.</summary>
        /// <param name="pipeSecurity">An action to configure access control and audit security that allows or denies new pipe client connections.</param>
        void Start(Action<PipeSecurity> pipeSecurity);

        /// <summary>Starts listening for new pipe client connections.</summary>
        /// <param name="pipeSecurity">Access control and audit security that allows or denies new pipe client connections.</param>
        void Start(PipeSecurity? pipeSecurity = default);

        /// <summary>Closes all client connections and stops listening for new connections.</summary>
        Task StopAsync();

        /// <summary>Asynchronously sends a message to the client with a specified pipe name..</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="pipeName">The name of the pipe to send the message to. This name is case-insensitive.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        Task WriteAsync(TMessage message, string pipeName, CancellationToken cancellationToken = default);

        /// <summary>Asynchronously sends a message to all connected clients that meet a predicate condition.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="predicate">The predicate condition to be met.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        Task WriteAsync(TMessage message, Func<INamedPipeConnection<TMessage>, bool> predicate, CancellationToken cancellationToken = default);
    }
}