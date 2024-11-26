using AllOverIt.Extensions;
using Microsoft.Extensions.Logging;

namespace AllOverIt.Logging.Testing
{
    /// <summary>Contains a collection of metadata related to logger method calls.</summary>
    public sealed class MethodCallContext : List<MethodCallContext.Item>
    {
        /// <summary>Contains metadata for a single log entry.</summary>
        public sealed record Item
        {
            /// <summary>The captured log level.</summary>
            public required LogLevel LogLevel { get; init; }

            /// <summary>The captured metadata, such as logger message templates and arguments, as key-value pairs.</summary>
            public required Dictionary<string, object?> Metadata { get; init; }

            /// <summary>The captured exception, if there is one.</summary>
            public required Exception? Exception { get; init; }
        }

        /// <summary>A collection of captured log levels.</summary>
        public LogLevel[] LogLevels => this.SelectToArray(item => item.LogLevel);

        /// <summary>A collection of metadata, such as logger message templates and arguments, as key-value pairs.</summary>
        public IDictionary<string, object?>[] Metadata => this.SelectToArray(item => item.Metadata);

        /// <summary>A collection of captured exceptions. A <see langword="null"/> value indicates no exception.</summary>
        public Exception?[] Exceptions => this.SelectToArray(item => item.Exception);
    }
}