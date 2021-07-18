namespace AllOverIt.Process
{
    /// <summary>Contains the output and exit status of a process executed via the <c>Process.ExecuteAndWaitAsync</c> method.</summary>
    public sealed class ProcessOutput
    {
        /// <summary>The exit code of the process.</summary>
        public int ExitCode { get; }

        /// <summary>The output captured from the standard output stream.</summary>
        public string StandardOutput { get; }

        /// <summary>The output captured from the standard error stream.</summary>
        public string StandardError { get; }

        /// <summary>Initializes a new <c>ProcessOutput</c> instance.</summary>
        /// <param name="exitCode">The exit code of the process.</param>
        /// <param name="standardOutput">The output captured from the standard output stream.</param>
        /// <param name="standardError">The output captured from the standard error stream.</param>
        public ProcessOutput(int exitCode, string standardOutput, string standardError)
        {
            ExitCode = exitCode;
            StandardOutput = standardOutput;
            StandardError = standardError;
        }
    }
}