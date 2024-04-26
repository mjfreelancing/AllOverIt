using AllOverIt.Assertion;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Process
{
    /// <summary>Specifies options used for configuring the execution of a process.</summary>
    [ExcludeFromCodeCoverage]
    public sealed record ProcessExecutorOptions     // A record so 'with' can be used to change state
    {
        /// <summary>The name of the process being executed.</summary>
        public string ProcessFileName { get; }

        /// <summary>The working directory of the process being executed.</summary>
        public string WorkingDirectory { get; init; }

        /// <summary>The arguments to be passed to the process being executed.</summary>
        public string Arguments { get; init; }

        /// <summary>Indicates whether to start the process in a new window.</summary>
        public bool NoWindow { get; init; }

        /// <summary>An execution timeout period for the process being executed. This property does not need
        /// to be set if the process is executed with an expiring <see cref="System.Threading.CancellationToken"/>.</summary>
        public TimeSpan Timeout { get; init; }

        /// <summary>Additional environment variables to be provided specifically to the process, and any child processes, being executed.</summary>
        public IDictionary<string, string> EnvironmentVariables { get; init; }

        /// <summary>A handler to receive standard output.</summary>
        public DataReceivedEventHandler StandardOutputHandler { get; init; }

        /// <summary>A handler to receive error output.</summary>
        public DataReceivedEventHandler ErrorOutputHandler { get; init; }

        /// <summary>Constructor.</summary>
        /// <param name="processFileName">The name of the process to be executed. This should be fully qualified if the process cannot be found on the system path.</param>
        public ProcessExecutorOptions(string processFileName)
        {
            ProcessFileName = processFileName.WhenNotNullOrEmpty(nameof(processFileName));
        }
    }
}
