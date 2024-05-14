using AllOverIt.Extensions;
using Microsoft.Extensions.Logging;

namespace AllOverIt.Logging.Testing
{
    /// <summary>Contains a collection of metadata related to logger method calls.</summary>
    public sealed class MethodCallContext : List<MethodCallContext.Item>
    {
        public sealed record Item
        {
            public LogLevel LogLevel { get; init; }
            public Dictionary<string, object> Metadata { get; init; }
            public Exception Exception { get; init; }
        }

        /// <summary>A collection of captured log levels.</summary>
        public LogLevel[] LogLevels => this.SelectToArray(item => item.LogLevel);

        /// <summary>A collection of metadata, such as logger message templates and arguments, as key-value pairs.</summary>
        public IDictionary<string, object>[] Metadata => this.SelectToArray(item => item.Metadata);

        /// <summary>A collection of captured exceptions. A <see langword="null"/> value indicates no exception.</summary>
        public Exception[] Exceptions => this.SelectToArray(item => item.Exception);
    }
}