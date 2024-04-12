using AllOverIt.Assertion;
using AllOverIt.Serilog.Sinks.CircularBuffer;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System;

namespace AllOverIt.Serilog.Extensions
{
    /// <summary>Provides extension methods for a <see cref="LoggerSinkConfiguration"/>.</summary>
    public static class LoggerSinkConfigurationExtensions
    {
        private const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message}{NewLine}{Exception}";

        /// <summary>Writes log events to a <see cref="ICircularBufferSinkMessages"/> instance.</summary>
        /// <param name="sinkConfiguration">The logger sink configuration.</param>
        /// <param name="sinkMessages">The circular buffer to receive the log event messages.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "<c>{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message}{NewLine}{Exception}</c>".</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for events passed through the sink.
        /// Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or <see langword="null"></see>.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
        /// <returns>The sink's <see cref="LoggerConfiguration"/> instance, allowing method chaining.</returns>
        public static LoggerConfiguration CircularBuffer(this LoggerSinkConfiguration sinkConfiguration,
            ICircularBufferSinkMessages sinkMessages,
            string outputTemplate = DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider? formatProvider = default,
            LoggingLevelSwitch? levelSwitch = default)
        {
            _ = sinkConfiguration.WhenNotNull(nameof(sinkConfiguration));
            _ = outputTemplate.WhenNotNullOrEmpty(nameof(outputTemplate));

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);

            return sinkConfiguration.CircularBuffer(sinkMessages, formatter, restrictedToMinimumLevel, levelSwitch);
        }

        /// <summary>Writes log events to a <see cref="ICircularBufferSinkMessages"/> instance.</summary>
        /// <param name="sinkConfiguration">The logger sink configuration.</param>
        /// <param name="sinkMessages">The circular buffer to receive the log event messages.</param>
        /// <param name="formatter">A custom formatter to apply to the log events. This can be used to produce, for example,
        /// <c>JSON</c> output. To customize the text layout only, use the
        /// <see cref="CircularBuffer(LoggerSinkConfiguration, ICircularBufferSinkMessages, string, LogEventLevel, IFormatProvider?, LoggingLevelSwitch?)"/>
        /// overload instead.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for events passed through the sink.
        /// Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
        /// <returns>The sink's <see cref="LoggerConfiguration"/> instance, allowing method chaining.</returns>
        public static LoggerConfiguration CircularBuffer(this LoggerSinkConfiguration sinkConfiguration,
            ICircularBufferSinkMessages sinkMessages,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch? levelSwitch = default)
        {
            _ = sinkConfiguration.WhenNotNull(nameof(sinkConfiguration));
            _ = formatter.WhenNotNull(nameof(formatter));

            var sink = new CircularBufferSink(sinkMessages, formatter);

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}
