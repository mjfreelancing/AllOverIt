using Microsoft.Extensions.Logging;

namespace AllOverIt.Logging.Testing
{
    public sealed record MethodCallContext
    {
        public LogLevel LogLevel { get; init; }
        public Dictionary<string, object> Entries { get; init; }
        public Exception Exception { get; init; }
    }
}