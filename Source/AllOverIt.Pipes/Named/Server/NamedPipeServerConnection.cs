using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Serialization;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Named.Server
{
    internal sealed class NamedPipeServerConnection<TMessage> : NamedPipeConnection<TMessage>, INamedPipeServerConnection<TMessage>
        where TMessage : class, new()
    {
        /// <inheritdoc />
        public event EventHandler<NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeServerConnection<TMessage>>> OnMessageReceived;

        /// <inheritdoc />
        public event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>> OnDisconnected;

        /// <inheritdoc />
        public event EventHandler<NamedPipeConnectionExceptionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>> OnException;

        /// <summary>Constructor.</summary>
        /// <param name="pipeStream">The underlying pipe stream.</param>
        /// <param name="connectionId">Gets the conection's unique identifier.</param>
        /// <param name="serializer">The message serializer.</param>
        public NamedPipeServerConnection(PipeStream pipeStream, string connectionId, INamedPipeSerializer<TMessage> serializer)
            : base(pipeStream, connectionId, serializer)
        {
        }

        /// <inheritdoc />
        public string GetImpersonationUserName()
        {
            if (PipeStream is not NamedPipeServerStream serverStream)
            {
                throw new PipeException($"The pipe stream must be a {nameof(NamedPipeServerStream)}.");
            }

            // IOException will be raised of the pipe connection has been broken or
            // the user name is longer than 19 characters.
            return serverStream.GetImpersonationUserName();
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
                    var args = new NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeServerConnection<TMessage>>(this, message);

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
                var args = new NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>(this);

                onDisconnected.Invoke(this, args);
            }
        }

        /// <summary>Raises an <see cref="OnException"/> event.</summary>
        protected override void DoOnException(Exception exception)
        {
            var onException = OnException;

            if (onException is not null)
            {
                var args = new NamedPipeConnectionExceptionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>(this, exception);

                onException.Invoke(this, args);
            }
        }
    }
}