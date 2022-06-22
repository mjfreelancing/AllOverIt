using AllOverIt.Async;
using AllOverIt.Process.Exceptions;
using System;
using System.Threading.Tasks;

namespace AllOverIt.Process
{
    /// <summary>Provides support for general process activities.</summary>
    public static class Process
    {
        /// <summary>Executes a process and captures the standard output and standard error streams.</summary>
        /// <param name="workingDirectory">The working directory to be used by the process to be executed</param>
        /// <param name="processFilename">The name of the executable to be spawned.</param>
        /// <param name="arguments">Arguments to be passed to the new process.</param>
        /// <param name="timeout">The period to wait for the process to exit before timing out.</param>
        /// <returns>The exit code, standard and error output of the executed process.</returns>
        public static Task<ProcessOutput> ExecuteAndWaitAsync(string workingDirectory, string processFilename, string arguments, TimeSpan timeout)
        {
            return ExecuteAndWaitAsync(workingDirectory, processFilename, arguments, (int)timeout.TotalMilliseconds);
        }

        /// <summary>Executes a process and captures the standard output and standard error streams.</summary>
        /// <param name="workingDirectory">The working directory to be used by the process to be executed</param>
        /// <param name="processFilename">The name of the executable to be spawned.</param>
        /// <param name="arguments">Arguments to be passed to the new process.</param>
        /// <param name="timeoutMilliseconds">The number of milliseconds to wait for the process to exit before timing out. Pass
        /// a value of -1 to wait indefinitely.</param>
        /// <returns>The exit code, standard and error output of the executed process.</returns>
        public static async Task<ProcessOutput> ExecuteAndWaitAsync(string workingDirectory, string processFilename, string arguments, int timeoutMilliseconds)
        {
            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.FileName = processFilename;
                process.StartInfo.Arguments = arguments;

                process.Start();

                // don't bother disposing of Task items - i.e., no need to wrap in using()
                // http://blogs.msdn.com/b/pfxteam/archive/2012/03/25/10287435.aspx

                var processWaiter = Task.Run(() => WaitForProcess(process, timeoutMilliseconds));
                var standardOutput = Task.Run(() => process.StandardOutput.ReadToEnd());
                var standardError = Task.Run(() => process.StandardError.ReadToEnd());

                var waitResult = false;
                Exception processException = null;

                try
                {
                    waitResult = await processWaiter;
                }

                catch (Exception exception)
                {
                    processException = exception;
                }

                if (!waitResult)
                {
                    process.Kill();
                }

                // wait for the reader tasks to complete
                var (outputResult, errorResult) = await TaskHelper.WhenAll(standardOutput, standardError);

                if (processException != null)
                {
                    throw new ProcessException("Process execution failed.", processException);
                }

                if (!waitResult)
                {
                    throw new TimeoutException("Process wait timeout expired.");
                }

                return new ProcessOutput(process.ExitCode, outputResult, errorResult);
            }
        }

        private static bool WaitForProcess(System.Diagnostics.Process process, int timeoutMilliseconds)
        {
            if (timeoutMilliseconds == -1)
            {
                process.WaitForExit();
                return true;
            }

            return process.WaitForExit(timeoutMilliseconds);
        }
    }
}
