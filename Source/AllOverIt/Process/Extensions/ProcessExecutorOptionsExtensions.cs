using AllOverIt.Assertion;
using AllOverIt.CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AllOverIt.Process.Extensions
{
    /// <summary>Provides extension methods for <see cref="ProcessExecutorOptions"/>.</summary>
    public static class ProcessExecutorOptionsExtensions
    {
        /// <summary>Creates a new <see cref="ProcessExecutorOptions"/> by cloning the provided <paramref name="options"/> and updating the working directory.</summary>
        /// <param name="options">The options to be cloned.</param>
        /// <param name="workingDirectory">The working directory to be applied to the cloned <see cref="ProcessExecutorOptions"/> instance.</param>
        /// <returns>A new <see cref="ProcessExecutorOptions"/> with an updated working directory.</returns>
        public static ProcessExecutorOptions WithWorkingDirectory(this ProcessExecutorOptions options, string workingDirectory)
        {
            return options with { WorkingDirectory = workingDirectory };
        }

        /// <summary>Creates a new <see cref="ProcessExecutorOptions"/> by cloning the provided <paramref name="options"/> and updating the arguments.</summary>
        /// <param name="options">The options to be cloned.</param>
        /// <param name="arguments">The arguments to be applied to the cloned <see cref="ProcessExecutorOptions"/> instance. Each argument will be
        /// escaped if required.</param>
        /// <returns>A new <see cref="ProcessExecutorOptions"/> with updated arguments.</returns>
        public static ProcessExecutorOptions WithArguments(this ProcessExecutorOptions options, params string[] arguments)
        {
            _ = arguments.WhenNotNull(nameof(arguments));

            return options.WithArguments([.. arguments], true);
        }

        /// <summary>Creates a new <see cref="ProcessExecutorOptions"/> by cloning the provided <paramref name="options"/> and updating the arguments.</summary>
        /// <param name="options">The options to be cloned.</param>
        /// <param name="arguments">The arguments to be applied to the cloned <see cref="ProcessExecutorOptions"/> instance.</param>
        /// <param name="escape">When <see langword="true" /> each argument will be escaped if required.</param>
        /// <returns>A new <see cref="ProcessExecutorOptions"/> with updated arguments.</returns>
        public static ProcessExecutorOptions WithArguments(this ProcessExecutorOptions options, IEnumerable<string> arguments, bool escape = true)
        {
            _ = arguments.WhenNotNull(nameof(arguments));

            var stringBuilder = new StringBuilder();

            foreach (var argument in arguments)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(' ');
                }

                stringBuilder.Append(escape ? Argument.Escape(argument) : argument);
            }

            return options with { Arguments = stringBuilder.ToString() };
        }

        /// <summary>Creates a new <see cref="ProcessExecutorOptions"/> by cloning the provided <paramref name="options"/> and updating the 'NoWindow' option.</summary>
        /// <param name="options">The options to be cloned.</param>
        /// <returns>A new <see cref="ProcessExecutorOptions"/> with its 'NoWindow' option set to <see langword="true"/>.</returns>
        public static ProcessExecutorOptions WithNoWindow(this ProcessExecutorOptions options)
        {
            return options with { NoWindow = true };
        }

        /// <summary>Creates a new <see cref="ProcessExecutorOptions"/> by cloning the provided <paramref name="options"/> and updating the timeout period.</summary>
        /// <param name="options">The options to be cloned.</param>
        /// <param name="milliseconds">The timeout period, in milliseconds.</param>
        /// <returns>A new <see cref="ProcessExecutorOptions"/> with an updated timeout.</returns>
        public static ProcessExecutorOptions WithTimeout(this ProcessExecutorOptions options, double milliseconds)
        {
            return options with { Timeout = TimeSpan.FromMilliseconds(milliseconds) };
        }

        /// <summary>Creates a new <see cref="ProcessExecutorOptions"/> by cloning the provided <paramref name="options"/> and updating the timeout period.</summary>
        /// <param name="options">The options to be cloned.</param>
        /// <param name="timespan">The timeout period, as a <see cref="TimeSpan"/>.</param>
        /// <returns>A new <see cref="ProcessExecutorOptions"/> with an updated timeout.</returns>
        public static ProcessExecutorOptions WithTimeout(this ProcessExecutorOptions options, TimeSpan timespan)
        {
            return options with { Timeout = timespan };
        }

        /// <summary>Creates a new <see cref="ProcessExecutorOptions"/> by cloning the provided <paramref name="options"/> and updating the environment variables.</summary>
        /// <param name="options">The options to be cloned.</param>
        /// <param name="environmentVariables">The environment variables to be provided specifically to the executed process, and any child processes.</param>
        /// <returns>A new <see cref="ProcessExecutorOptions"/> with updated environment variables.</returns>
        public static ProcessExecutorOptions WithEnvironmentVariables(this ProcessExecutorOptions options, IDictionary<string, string> environmentVariables)
        {
            _ = environmentVariables.WhenNotNull(nameof(environmentVariables));

            return options with { EnvironmentVariables = environmentVariables };
        }

        /// <summary>Creates a new <see cref="ProcessExecutorOptions"/> by cloning the provided <paramref name="options"/> and updating the environment variables.</summary>
        /// <param name="options">The options to be cloned.</param>
        /// <param name="configure">An action to configure environment variables to be provided specifically to the executed process, and any child processes.
        /// This method can be called multiple times to append new environment variables.</param>
        /// <returns>A new <see cref="ProcessExecutorOptions"/> with updated environment variables.</returns>
        public static ProcessExecutorOptions WithEnvironmentVariables(this ProcessExecutorOptions options, Action<IDictionary<string, string>> configure)
        {
            _ = configure.WhenNotNull(nameof(configure));

            // Supports chaining multiple calls together
            var environmentVariables = options.EnvironmentVariables ?? new Dictionary<string, string>();

            configure.Invoke(environmentVariables);

            return options.WithEnvironmentVariables(environmentVariables);
        }

        /// <summary>Creates a new <see cref="ProcessExecutorOptions"/> by cloning the provided <paramref name="options"/> and updating the handler used for
        /// collecting standard output.</summary>
        /// <param name="options">The options to be cloned.</param>
        /// <param name="standardOutputHandler">The handler that will receive standard output generated by the executing process.</param>
        /// <returns>A new <see cref="ProcessExecutorOptions"/> with an updated standard output handler.</returns>
        public static ProcessExecutorOptions WithStandardOutputHandler(this ProcessExecutorOptions options, DataReceivedEventHandler standardOutputHandler)
        {
            return options with { StandardOutputHandler = standardOutputHandler };
        }

        /// <summary>Creates a new <see cref="ProcessExecutorOptions"/> by cloning the provided <paramref name="options"/> and updating the handler used for
        /// collecting error output.</summary>
        /// <param name="options">The options to be cloned.</param>
        /// <param name="errorOutputHandler">The handler that will receive error output generated by the executing process.</param>
        /// <returns>A new <see cref="ProcessExecutorOptions"/> with an updated error output handler.</returns>
        public static ProcessExecutorOptions WithErrorOutputHandler(this ProcessExecutorOptions options, DataReceivedEventHandler errorOutputHandler)
        {
            return options with { ErrorOutputHandler = errorOutputHandler };
        }

        /// <summary>Creates a process executor that is configured using the provided <paramref name="options"/>.</summary>
        /// <param name="options">The options used to configure the process executor.</param>
        /// <returns>A new instance of a process executor.</returns>
        public static IProcessExecutor BuildProcessExecutor(this ProcessExecutorOptions options)
        {
            return new ProcessExecutor(options);
        }

        /// <summary>Starts a configured process without waiting for it to exit.</summary>
        /// <returns>The backing <see cref="System.Diagnostics.Process"/> that has been started.</returns>
        [ExcludeFromCodeCoverage]
        public static System.Diagnostics.Process Start(this ProcessExecutorOptions options)
        {
            var process = ProcessFactory.CreateProcess(options);

            process.Start();

            return process;
        }
    }
}
