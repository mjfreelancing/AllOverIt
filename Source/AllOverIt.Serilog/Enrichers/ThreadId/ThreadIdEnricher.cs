using Serilog.Core;
using Serilog.Events;

namespace AllOverIt.Serilog.Enrichers.ThreadId
{
    /// <summary>An enricher that adds the current thread Id to the log event as a property.</summary>
    public class ThreadIdEnricher : ILogEventEnricher
    {
        /// <inheritdoc/>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var property = propertyFactory.CreateProperty("ThreadId", Environment.CurrentManagedThreadId);

            logEvent.AddPropertyIfAbsent(property);
        }
    }
}
