using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Process
{
    /// <summary>Provides process exit information along with buffered standard and error output at the completion of execution.</summary>
    [ExcludeFromCodeCoverage]
    public sealed class ProcessExecutorBufferedResult : ProcessExecutorResult
    {
        /// <summary>A copy of the standard output produced by the executing process.</summary>
        public string StandardOutput { get; }

        /// <summary>A copy of the error output produced by the executing process.</summary>
        public string ErrorOutput { get; }

        internal ProcessExecutorBufferedResult(System.Diagnostics.Process process, string standardOutput, string errorOutput)
            : base(process)
        {
            StandardOutput = standardOutput;
            ErrorOutput = errorOutput;
        }
    }
}
