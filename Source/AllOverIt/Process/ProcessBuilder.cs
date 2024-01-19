using AllOverIt.Assertion;

namespace AllOverIt.Process
{
    /// <summary>A builder for configuring a <see cref="ProcessExecutor"/> that can be later executed.</summary>
    public static class ProcessBuilder
    {
        /// <summary>Creates a default <see cref="ProcessExecutorOptions"/> for the provided process filename.</summary>
        /// <param name="processFileName">The filename for the process to be executed. This should include a fully
        /// qualified file path if the executable cannot be found on the system path.</param>
        /// <returns>A default <see cref="ProcessExecutorOptions"/> for the provided process filename.</returns>
        public static ProcessExecutorOptions For(string processFileName)
        {
            _ = processFileName.WhenNotNullOrEmpty(nameof(processFileName));

            return new ProcessExecutorOptions(processFileName);
        }
    }
}
