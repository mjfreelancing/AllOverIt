using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Process
{
    /// <summary>Provides process exit information at the completion of execution.</summary>
    [ExcludeFromCodeCoverage]
    public class ProcessExecutorResult
    {
        /// <summary>The start time of the executed process.</summary>
        public DateTime StartTime { get; }

        /// <summary>The exit time of the executed process.</summary>
        public DateTime ExitTime { get; }

        /// <summary>The exit code of the executed process.</summary>
        public int ExitCode { get; }

        /// <summary>The total running time of the executed process.</summary>
        public TimeSpan RunTime { get; }

        internal ProcessExecutorResult(System.Diagnostics.Process process)
        {
            StartTime = process.StartTime;
            ExitTime = process.ExitTime;
            ExitCode = process.ExitCode;
            RunTime = ExitTime - StartTime;
        }
    }
}
