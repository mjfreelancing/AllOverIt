namespace AllOverIt.GenericHost
{
    /// <summary>An abstract base class for a hosted console application.</summary>
    public abstract class ConsoleAppBase : GenericAppBase, IConsoleApp
    {
        /// <inheritdoc />
        /// <remarks>The default exit code is 0.</remarks>
        public int DefaultExitCode { get; protected set; } = 0;

        /// <inheritdoc />
        /// <remarks>The default unhandled error exit code is -1.</remarks>
        public int UnhandedErrorExitCode { get; protected set; } = -1;

        /// <inheritdoc />
        /// <remarks>If this value is not set then the <see cref="DefaultExitCode"/> will be returned by the application when it exits.</remarks>
        public int? ExitCode { get; protected set; }
    }
}