using AllOverIt.Assertion;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace AllOverIt.Serilog.Sinks.CircularBuffer
{
    /// <summary>A destination for log events that are stored in a fixed-sized circular buffer.</summary>
    public class CircularBufferSink : ILogEventSink
    {
        // TODO: Update when NET 8 is no longer supported
#if NET9_0_OR_GREATER
        private readonly Lock _syncRoot = new();
#else
        private readonly object _syncRoot = new();
#endif

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

                lock (_syncRoot)
                {
                    _sinkMessages.PushBack(bufferMessage);
                }
            }
        }
    }
}
