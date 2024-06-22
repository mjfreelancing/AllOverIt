using Serilog.Core;
using Serilog.Events;

namespace AllOverIt.Serilog.Sinks.Observable
{
    /// <summary>Represents an observable sink for log events.</summary>
    public interface IObservableSink : IObservable<LogEvent>, ILogEventSink
    {
        /// <summary>Gets the current count of subscribers.</summary>
        int Count { get; }
    }
}
