using Serilog.Core;
using Serilog.Events;

namespace AllOverIt.Serilog.Enrichers.ThreadId
{
    /// <summary>An enricher that adds the current thread Id to the log event as a property.</summary>
    public class ThreadIdEnricher : ILogEventEnricher
    {
        /// <summary>The property name associated with the ThreadId added to the log event.</summary>
        public const string ThreadIdPropertyName = "ThreadId";

        /// <inheritdoc/>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var property = propertyFactory.CreateProperty(ThreadIdPropertyName, Environment.CurrentManagedThreadId);

            logEvent.AddPropertyIfAbsent(property);
        }
    }
}
