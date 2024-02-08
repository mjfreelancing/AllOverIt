using Serilog.Events;

namespace AllOverIt.Serilog.Sinks.CircularBuffer
{
    /// <summary>Contains an individual log event and a formatted version of the text message.</summary>
    /// <param name="LogEvent">The log event.</param>
    /// <param name="FormattedMessage">A formatted version of the text message, as per the sink's format configuration.</param>
    public sealed record CircularBufferSinkMessage(LogEvent LogEvent, string FormattedMessage);
}
