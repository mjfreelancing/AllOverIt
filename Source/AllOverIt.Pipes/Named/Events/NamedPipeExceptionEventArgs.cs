using AllOverIt.Assertion;

namespace AllOverIt.Pipes.Named.Events
{
    /// <summary>Defines named pipe exception related event arguments.</summary>
    public class NamedPipeExceptionEventArgs : EventArgs
    {
        /// <summary>The exception that was raised.</summary>
        public Exception Exception { get; }

        /// <summary>Constructor.</summary>
        /// <param name="exception">The exception associated with the event.</param>
        public NamedPipeExceptionEventArgs(Exception exception)
        {
            Exception = exception.WhenNotNull();
        }
    }
}