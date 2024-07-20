using AllOverIt.Assertion;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace AllOverIt.Serilog.Sinks.CircularBuffer
{
    /// <summary>A destination for log events that are stored in a fixed-sized circular buffer.</summary>
    public class CircularBufferSink : ILogEventSink
    {
        private readonly ICircularBufferSinkMessages _sinkMessages;
        private readonly ITextFormatter _formatter;

        /// <summary>Constructor.</summary>
        /// <param name="sinkMessages">The circular buffer to receive the log event messages.</param>
        /// <param name="formatter">A custom formatter to apply to the log events.</param>
        public CircularBufferSink(ICircularBufferSinkMessages sinkMessages, ITextFormatter formatter)
        {
            _sinkMessages = sinkMessages.WhenNotNull();
            _formatter = formatter.WhenNotNull();
        }

        /// <inheritdoc/>
        public void Emit(LogEvent logEvent)
        {
            // Assuming serilog will never send a null LogEvent

            using (var writer = new StringWriter())
            {
                _formatter.Format(logEvent, writer);

                var formattedMessage = writer.ToString().Trim();

                var bufferMessage = new CircularBufferSinkMessage(logEvent, formattedMessage);

                lock (_sinkMessages)
                {
                    _sinkMessages.PushBack(bufferMessage);
                }
            }
        }
    }
}
