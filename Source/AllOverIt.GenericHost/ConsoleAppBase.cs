namespace AllOverIt.GenericHost
{
    /// <summary>An abstract base class for a hosted console application.</summary>
    public abstract class ConsoleAppBase : GenericAppBase, IConsoleApp
    {
        // Written from a thread-pool thread (Console.CancelKeyPress) and read by the host during shutdown.
        private volatile bool _userCancelled;

        /// <inheritdoc />
        /// <remarks>The default exit code is 0.</remarks>
        public int DefaultExitCode { get; protected set; } = 0;

        /// <inheritdoc />
        /// <remarks>The default unhandled error exit code is -1.</remarks>
        public int UnhandedErrorExitCode { get; protected set; } = -1;

        /// <inheritdoc />
        /// <remarks>The default cancelled exit code is -2.</remarks>
        public int CancelledExitCode { get; protected set; } = -2;

        /// <inheritdoc />
        /// <remarks>If this value is not set then the <see cref="DefaultExitCode"/> will be returned by the application when it exits.</remarks>
        public int? ExitCode { get; protected set; }

        /// <inheritdoc />
        public bool UserCancelled => _userCancelled;

        /// <summary>Initializes a new instance of the <see cref="ConsoleAppBase"/> class.</summary>
        protected ConsoleAppBase()
        {
            // The runtime raises this event on Ctrl+C / Ctrl+Break (Windows) and SIGINT (Unix). It runs on a
            // thread-pool thread. The host's ConsoleLifetime registers its own handler (which cancels the
            // default termination and triggers graceful shutdown) - here we only record the cause.
            Console.CancelKeyPress += (_, _) => _userCancelled = true;
        }
    }
}