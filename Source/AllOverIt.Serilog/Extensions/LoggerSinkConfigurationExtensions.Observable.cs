using AllOverIt.Assertion;
using AllOverIt.Serilog.Sinks.Observable;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace AllOverIt.Serilog.Extensions
{
    /// <summary>Provides extension methods for a <see cref="LoggerSinkConfiguration"/>.</summary>
    public static partial class LoggerSinkConfigurationExtensions
    {
        /// <summary>Writes log events to reactive observers.</summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="observableSink">The observable sink to emit all log events.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for events passed through the sink.
        /// Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
        /// <returns>The sink's <see cref="LoggerConfiguration"/> instance, allowing method chaining.</returns>
        public static LoggerConfiguration Observable(
            this LoggerSinkConfiguration sinkConfiguration,
            IObservableSink observableSink,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch? levelSwitch = null)
        {
            _ = sinkConfiguration.WhenNotNull();
            _ = observableSink.WhenNotNull();

            return sinkConfiguration.Sink(observableSink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}
