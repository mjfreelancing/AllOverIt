using AllOverIt.Assertion;
using Serilog.Events;

namespace AllOverIt.Serilog.Sinks.CircularBuffer
{
    /// <summary>Contains an individual log event and a formatted version of the text message.</summary>
    public sealed class CircularBufferSinkMessage
    {
        /// <summary>The log event.</summary>
        public LogEvent LogEvent { get; }

        /// <summary>A formatted version of the text message, as per the sink's format configuration.</summary>
        public string FormattedMessage { get; }

        /// <summary>Constructor.</summary>
        /// <param name="logEvent">The log event.</param>
        /// <param name="formattedMessage">A formatted version of the text message, as per the sink's format configuration.</param>
        public CircularBufferSinkMessage(LogEvent logEvent, string formattedMessage)
        {
            LogEvent = logEvent.WhenNotNull(nameof(logEvent));
            FormattedMessage = formattedMessage.WhenNotNullOrEmpty(nameof(formattedMessage));
        }
    }
}
