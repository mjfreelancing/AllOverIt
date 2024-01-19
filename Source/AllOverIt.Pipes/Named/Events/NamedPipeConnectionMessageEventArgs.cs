using AllOverIt.Assertion;
using AllOverIt.Pipes.Named.Connection;

namespace AllOverIt.Pipes.Named.Events
{
    /// <summary>Defines named pipe message related event arguments.</summary>
    public sealed class NamedPipeConnectionMessageEventArgs<TMessage, TPipeConnection> : NamedPipeConnectionEventArgs<TMessage, TPipeConnection>
        where TPipeConnection : class, INamedPipeConnection<TMessage>
        where TMessage : class, new()
    {
        /// <summary>The message sent by the other end of the pipe.</summary>
        public TMessage Message { get; }

        /// <summary>Constructor.</summary>
        /// <param name="connection">The connection associated with the event.</param>
        /// <param name="message">The message associated with the event.</param>
        public NamedPipeConnectionMessageEventArgs(TPipeConnection connection, TMessage message)
            : base(connection)
        {
            Message = message.WhenNotNull();
        }
    }
}