using AllOverIt.Assertion;
using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Serialization;
using System;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Named.Client
{
    /// <summary>Implements a named pipe client connection.</summary>
    /// <typeparam name="TMessage">The message type serialized between a named pipe client and a named pipe server.</typeparam>
    internal sealed class NamedPipeClientConnection<TMessage> : NamedPipeConnection<TMessage>, INamedPipeClientConnection<TMessage>
        where TMessage : class, new()
    {
        /// <inheritdoc />
        public event EventHandler<NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnMessageReceived;

        /// <inheritdoc />
        public event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnDisconnected;

        /// <inheritdoc />
        public event EventHandler<NamedPipeConnectionExceptionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnException;

        /// <inheritdoc />
        public string ServerName { get; }

        /// <summary>Constructor.</summary>
        /// <param name="pipeStream">The underlying pipe stream.</param>
        /// <param name="connectionId">Gets the conection's unique identifier.</param>
        /// <param name="serverName">The name of the server to communicate with.</param>
        /// <param name="serializer">The serializer to be used by named pipe client instances.</param>
        public NamedPipeClientConnection(PipeStream pipeStream, string connectionId, string serverName, INamedPipeSerializer<TMessage> serializer)
            : base(pipeStream, connectionId, serializer)
        {
            ServerName = serverName.WhenNotNullOrEmpty(nameof(serverName));
        }

        /// <summary>Raises an <see cref="OnMessageReceived"/> event.</summary>
        /// <param name="message">The message received.</param>
        protected override void DoOnMessageReceived(TMessage message)
        {
            var onMessageReceived = OnMessageReceived;

            if (onMessageReceived is not null)
            {
                try
                {
                    var args = new NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeClientConnection<TMessage>>(this, message);

                    onMessageReceived.Invoke(this, args);
                }
                catch (Exception ex)
                {
                    DoOnException(ex);
                }
            }
        }

        /// <summary>Raises an <see cref="OnDisconnected"/> event.</summary>
        protected override void DoOnDisconnected()
        {
            var onDisconnected = OnDisconnected;

            if (onDisconnected is not null)
            {
                var args = new NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>(this);

                onDisconnected.Invoke(this, args);
            }
        }

        /// <summary>Raises an <see cref="OnException"/> event.</summary>
        protected override void DoOnException(Exception exception)
        {
            var onException = OnException;

            if (onException is not null)
            {
                var args = new NamedPipeConnectionExceptionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>(this, exception);

                onException.Invoke(this, args);
            }
        }
    }
}