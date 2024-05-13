using AllOverIt.Extensions;
using Microsoft.Extensions.Logging;

namespace AllOverIt.Logging.Testing
{
    public sealed class MethodCallContext : List<MethodCallContext.Item>
    {
        public sealed record Item
        {
            public LogLevel LogLevel { get; init; }
            public Dictionary<string, object> Entries { get; init; }
            public Exception Exception { get; init; }
        }

        public LogLevel[] LogLevels => this.SelectToArray(item => item.LogLevel);
        public IDictionary<string, object>[] Entries => this.SelectToArray(item => item.Entries);
        public Exception[] Exceptions => this.SelectToArray(item => item.Exception);
    }
}